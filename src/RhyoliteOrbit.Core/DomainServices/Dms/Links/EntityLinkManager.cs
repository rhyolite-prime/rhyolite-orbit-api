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

namespace RhyoliteOrbit.DomainServices.Dms.Links;

public interface IEntityLinkManager
{
    Task<DmsEntityLink> LinkAsync(RhyoliteOrbitConsts.DmsEntityType entityType, string entityId, Guid documentId, RhyoliteOrbitConsts.DmsLinkType linkType, string note, long createdBy);

    Task UnlinkAsync(Guid linkId, long removedBy);

    Task UnlinkDocumentAsync(RhyoliteOrbitConsts.DmsEntityType entityType, string entityId, Guid documentId, long removedBy);

    Task<List<DmsEntityLink>> GetForEntityAsync(RhyoliteOrbitConsts.DmsEntityType entityType, string entityId);

    Task<List<DmsEntityLink>> GetForDocumentAsync(Guid documentId);
}

/// <summary>
/// Links documents to records of other ERP modules so any invoice, payslip,
/// student, order or non conformance can carry its supporting documents.
/// </summary>
public class EntityLinkManager: DomainService, IEntityLinkManager
{
    private readonly IRepository<DmsEntityLink, Guid> _repository;
    private readonly IRepository<DmsDocument, Guid> _documentRepository;
    private readonly IAbpSession _abpSession;

    public EntityLinkManager(
        IRepository<DmsEntityLink, Guid> repository,
        IRepository<DmsDocument, Guid> documentRepository,
        IAbpSession abpSession)
    {
        _repository = repository;
        _documentRepository = documentRepository;
        _abpSession = abpSession;
    }

    public async Task<DmsEntityLink> LinkAsync(
        RhyoliteOrbitConsts.DmsEntityType entityType,
        string entityId,
        Guid documentId,
        RhyoliteOrbitConsts.DmsLinkType linkType,
        string note,
        long createdBy)
    {
        var document = await _documentRepository.FirstOrDefaultAsync(documentId);
        if (document == null)
        {
            throw new UserFriendlyException("DmsDocumentNotFound", "The requested document was not found.");
        }

        var existing = await (await _repository.GetAllAsync())
            .FirstOrDefaultAsync(l =>
                l.EntityType == (int)entityType &&
                l.EntityId == entityId &&
                l.DocumentId == documentId &&
                l.LinkType == (int)linkType);

        if (existing != null)
        {
            return existing;
        }

        var link = new DmsEntityLink
        {
            TenantId = _abpSession.TenantId ?? 0,
            EntityType = (int)entityType,
            EntityId = entityId,
            DocumentId = documentId,
            LinkType = (int)linkType,
            Note = note,
            CreatedById = createdBy
        };

        return await _repository.InsertAsync(link);
    }

    public async Task UnlinkAsync(Guid linkId, long removedBy)
    {
        await _repository.HardDeleteAsync(linkId);
    }

    public async Task UnlinkDocumentAsync(RhyoliteOrbitConsts.DmsEntityType entityType, string entityId, Guid documentId, long removedBy)
    {
        var links = await (await _repository.GetAllAsync())
            .Where(l => l.EntityType == (int)entityType && l.EntityId == entityId && l.DocumentId == documentId)
            .ToListAsync();

        foreach (var link in links)
        {
            await _repository.HardDeleteAsync(link);
        }
    }

    public async Task<List<DmsEntityLink>> GetForEntityAsync(RhyoliteOrbitConsts.DmsEntityType entityType, string entityId)
    {
        return await (await _repository.GetAllAsync())
            .Where(l => l.EntityType == (int)entityType && l.EntityId == entityId)
            .OrderByDescending(l => l.CreationTime)
            .ToListAsync();
    }

    public async Task<List<DmsEntityLink>> GetForDocumentAsync(Guid documentId)
    {
        return await (await _repository.GetAllAsync())
            .Where(l => l.DocumentId == documentId)
            .ToListAsync();
    }
}
