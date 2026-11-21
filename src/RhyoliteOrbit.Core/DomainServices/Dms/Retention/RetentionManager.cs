using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Dms;

namespace RhyoliteOrbit.DomainServices.Dms.Retention;

public interface IRetentionManager
{
    Task<DmsRetentionPolicy> CreatePolicyAsync(DmsRetentionPolicy policy);

    Task<DmsRetentionPolicy> UpdatePolicyAsync(DmsRetentionPolicy policy);

    Task DeletePolicyAsync(Guid policyId);

    Task<List<DmsRetentionPolicy>> ListPoliciesAsync(bool onlyActive = false);

    Task ApplyPolicyAsync(Guid documentId, Guid policyId, DateTime? eventDate, long appliedBy);

    Task<List<DmsDocument>> GetDispositionQueueAsync(int pageNo, int pageSize);

    Task<int> ExecuteDispositionAsync(long executedBy);

    Task<int> PurgeExpiredTrashAsync(int trashRetentionDays);
}

/// <summary>
/// Records management: computes retention expiry and executes the disposition
/// action (review, delete, archive, transfer).
/// </summary>
public class RetentionManager: DomainService, IRetentionManager
{
    private readonly IRepository<DmsRetentionPolicy, Guid> _policyRepository;
    private readonly IRepository<DmsDocument, Guid> _documentRepository;
    private readonly IAbpSession _abpSession;

    public RetentionManager(
        IRepository<DmsRetentionPolicy, Guid> policyRepository,
        IRepository<DmsDocument, Guid> documentRepository,
        IAbpSession abpSession)
    {
        _policyRepository = policyRepository;
        _documentRepository = documentRepository;
        _abpSession = abpSession;
    }

    public async Task<DmsRetentionPolicy> CreatePolicyAsync(DmsRetentionPolicy policy)
    {
        policy.TenantId = _abpSession.TenantId ?? 0;
        policy.IsActive = true;

        return await _policyRepository.InsertAsync(policy);
    }

    public async Task<DmsRetentionPolicy> UpdatePolicyAsync(DmsRetentionPolicy policy)
    {
        var existing = await _policyRepository.FirstOrDefaultAsync(policy.Id);
        if (existing == null)
        {
            throw new UserFriendlyException("The retention policy was not found.");
        }

        existing.Name = policy.Name;
        existing.Description = policy.Description;
        existing.Trigger = policy.Trigger;
        existing.RetentionDays = policy.RetentionDays;
        existing.DispositionAction = policy.DispositionAction;
        existing.AllowLegalHold = policy.AllowLegalHold;
        existing.IsActive = policy.IsActive;
        existing.FolderIds = policy.FolderIds;

        return await _policyRepository.UpdateAsync(existing);
    }

    public async Task DeletePolicyAsync(Guid policyId)
    {
        await _policyRepository.HardDeleteAsync(policyId);
    }

    public async Task<List<DmsRetentionPolicy>> ListPoliciesAsync(bool onlyActive = false)
    {
        var query = await _policyRepository.GetAllAsync();

        if (onlyActive)
        {
            query = query.Where(p => p.IsActive);
        }

        return await query.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task ApplyPolicyAsync(Guid documentId, Guid policyId, DateTime? eventDate, long appliedBy)
    {
        var policy = await _policyRepository.FirstOrDefaultAsync(policyId);
        if (policy == null)
        {
            throw new UserFriendlyException("The retention policy was not found.");
        }

        var document = await _documentRepository.FirstOrDefaultAsync(documentId);
        if (document == null)
        {
            throw new UserFriendlyException("DmsDocumentNotFound", "The requested document was not found.");
        }

        document.RetentionPolicyId = policy.Id;
        document.RetentionEventDate = eventDate ?? DateTime.UtcNow;

        await _documentRepository.UpdateAsync(document);
    }

    public async Task<List<DmsDocument>> GetDispositionQueueAsync(int pageNo, int pageSize)
    {
        if (pageNo < 1)
        {
            pageNo = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 20;
        }

        var policies = await ListPoliciesAsync(true);
        var policyMap = policies.ToDictionary(p => p.Id, p => p);

        var now = DateTime.UtcNow;

        var documents = await (await _documentRepository.GetAllAsync())
            .Where(d => !d.IsDeleted
                        && d.Status != (int)RhyoliteOrbitConsts.DocumentStatus.Archived
                        && d.RetentionPolicyId != null
                        && !d.IsOnLegalHold)
            .ToListAsync();

        var due = documents
            .Where(d => policyMap.TryGetValue(d.RetentionPolicyId.Value, out var policy) && IsDue(d, policy, now))
            .OrderBy(d => d.CreationTime)
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return due;
    }

    public async Task<int> ExecuteDispositionAsync(long executedBy)
    {
        var policies = await ListPoliciesAsync(true);
        var policyMap = policies.ToDictionary(p => p.Id, p => p);
        var now = DateTime.UtcNow;

        var documents = await (await _documentRepository.GetAllAsync())
            .Where(d => !d.IsDeleted && d.RetentionPolicyId != null && !d.IsOnLegalHold && !d.IsWorm)
            .ToListAsync();

        var processed = 0;

        foreach (var document in documents)
        {
            if (!policyMap.TryGetValue(document.RetentionPolicyId.Value, out var policy))
            {
                continue;
            }

            if (!IsDue(document, policy, now))
            {
                continue;
            }

            switch ((RhyoliteOrbitConsts.DispositionAction)policy.DispositionAction)
            {
                case RhyoliteOrbitConsts.DispositionAction.Delete:
                    document.IsDeleted = true;
                    document.DeletionTime = now;
                    document.DeleterUserId = executedBy;
                    document.Status = (int)RhyoliteOrbitConsts.DocumentStatus.Deleted;
                    await _documentRepository.UpdateAsync(document);
                    break;

                case RhyoliteOrbitConsts.DispositionAction.Archive:
                    document.Status = (int)RhyoliteOrbitConsts.DocumentStatus.Archived;
                    await _documentRepository.UpdateAsync(document);
                    break;

                case RhyoliteOrbitConsts.DispositionAction.Review:
                case RhyoliteOrbitConsts.DispositionAction.Transfer:
                    // kept for a records officer to action manually
                    continue;
            }

            processed++;
        }

        return processed;
    }

    public async Task<int> PurgeExpiredTrashAsync(int trashRetentionDays)
    {
        if (trashRetentionDays <= 0)
        {
            trashRetentionDays = 30;
        }

        var threshold = DateTime.UtcNow.AddDays(-trashRetentionDays);

        var expired = await (await _documentRepository.GetAllAsync())
            .IgnoreQueryFilters()
            .Where(d => d.IsDeleted && d.DeletionTime != null && d.DeletionTime < threshold && !d.IsOnLegalHold && !d.IsWorm)
            .ToListAsync();

        foreach (var document in expired)
        {
            await _documentRepository.HardDeleteAsync(document);
        }

        return expired.Count;
    }

    private static bool IsDue(DmsDocument document, DmsRetentionPolicy policy, DateTime now)
    {
        var anchor = (RhyoliteOrbitConsts.RetentionTrigger)policy.Trigger switch
        {
            RhyoliteOrbitConsts.RetentionTrigger.LastModified => document.LastModificationTime ?? document.CreationTime,
            RhyoliteOrbitConsts.RetentionTrigger.EventDate => document.RetentionEventDate ?? document.CreationTime,
            _ => document.CreationTime
        };

        return anchor.AddDays(policy.RetentionDays) <= now;
    }
}
