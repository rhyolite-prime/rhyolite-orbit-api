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

public interface ILegalHoldManager
{
    Task<DmsLegalHold> ApplyAsync(DmsLegalHold hold);

    Task ReleaseAsync(Guid holdId, long releasedBy);

    Task<List<DmsLegalHold>> ListAsync(bool onlyActive = true);

    Task<List<DmsDocument>> GetHeldDocumentsAsync();

    Task<bool> IsHeldAsync(Guid documentId);
}

/// <summary>
/// Legal holds suspend disposition and deletion for documents and folders.
/// </summary>
public class LegalHoldManager: DomainService, ILegalHoldManager
{
    private readonly IRepository<DmsLegalHold, Guid> _holdRepository;
    private readonly IRepository<DmsDocument, Guid> _documentRepository;
    private readonly IAbpSession _abpSession;

    public LegalHoldManager(
        IRepository<DmsLegalHold, Guid> holdRepository,
        IRepository<DmsDocument, Guid> documentRepository,
        IAbpSession abpSession)
    {
        _holdRepository = holdRepository;
        _documentRepository = documentRepository;
        _abpSession = abpSession;
    }

    public async Task<DmsLegalHold> ApplyAsync(DmsLegalHold hold)
    {
        hold.TenantId = _abpSession.TenantId ?? 0;
        hold.IsActive = true;
        hold.AppliedById = _abpSession.UserId ?? 0;
        hold.DocumentIds = hold.DocumentIds ?? new List<Guid>();
        hold.FolderIds = hold.FolderIds ?? new List<Guid>();

        var created = await _holdRepository.InsertAsync(hold);

        await RefreshDocumentFlagsAsync();

        return created;
    }

    public async Task ReleaseAsync(Guid holdId, long releasedBy)
    {
        var hold = await _holdRepository.FirstOrDefaultAsync(holdId);
        if (hold == null)
        {
            throw new UserFriendlyException("The legal hold was not found.");
        }

        hold.IsActive = false;
        hold.ReleasedAt = DateTime.UtcNow;

        await _holdRepository.UpdateAsync(hold);

        await RefreshDocumentFlagsAsync();
    }

    public async Task<List<DmsLegalHold>> ListAsync(bool onlyActive = true)
    {
        var query = await _holdRepository.GetAllAsync();

        if (onlyActive)
        {
            query = query.Where(h => h.IsActive);
        }

        return await query.OrderByDescending(h => h.CreationTime).ToListAsync();
    }

    public async Task<List<DmsDocument>> GetHeldDocumentsAsync()
    {
        var holds = await ListAsync(true);

        var documentIds = holds.SelectMany(h => h.DocumentIds ?? new List<Guid>()).Distinct().ToList();
        var folderIds = holds.SelectMany(h => h.FolderIds ?? new List<Guid>()).Distinct().ToList();

        var documents = await (await _documentRepository.GetAllAsync())
            .Where(d => documentIds.Contains(d.Id) || folderIds.Contains(d.FolderId))
            .ToListAsync();

        return documents;
    }

    public async Task<bool> IsHeldAsync(Guid documentId)
    {
        var document = await _documentRepository.FirstOrDefaultAsync(documentId);
        if (document == null)
        {
            return false;
        }

        if (document.IsOnLegalHold)
        {
            return true;
        }

        var holds = await ListAsync(true);
        var folderIds = holds.SelectMany(h => h.FolderIds ?? new List<Guid>()).ToList();

        return folderIds.Contains(document.FolderId);
    }

    private async Task RefreshDocumentFlagsAsync()
    {
        var holds = await ListAsync(true);
        var documentIds = holds.SelectMany(h => h.DocumentIds ?? new List<Guid>()).Distinct().ToList();
        var folderIds = holds.SelectMany(h => h.FolderIds ?? new List<Guid>()).Distinct().ToList();

        var all = await (await _documentRepository.GetAllAsync()).ToListAsync();

        foreach (var document in all)
        {
            var held = documentIds.Contains(document.Id) || folderIds.Contains(document.FolderId);

            if (document.IsOnLegalHold != held)
            {
                document.IsOnLegalHold = held;
                await _documentRepository.UpdateAsync(document);
            }
        }
    }
}
