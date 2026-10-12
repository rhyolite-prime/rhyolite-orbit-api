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

namespace RhyoliteOrbit.DomainServices.Dms.Annotations;

public interface IAnnotationManager
{
    Task<DmsAnnotation> CreateAsync(DmsAnnotation annotation);

    Task DeleteAsync(Guid annotationId, long deletedBy);

    Task<List<DmsAnnotation>> ListAsync(Guid documentId, Guid? versionId = null);

    Task<List<DmsAnnotation>> ListRedactionsAsync(Guid documentId);
}

public class AnnotationManager: DomainService, IAnnotationManager
{
    private readonly IRepository<DmsAnnotation, Guid> _repository;
    private readonly IAbpSession _abpSession;

    public AnnotationManager(IRepository<DmsAnnotation, Guid> repository, IAbpSession abpSession)
    {
        _repository = repository;
        _abpSession = abpSession;
    }

    public async Task<DmsAnnotation> CreateAsync(DmsAnnotation annotation)
    {
        annotation.TenantId = _abpSession.TenantId ?? 0;

        if (annotation.Type == (int)RhyoliteOrbitConsts.AnnotationType.Redaction)
        {
            annotation.IsRedaction = true;
        }

        return await _repository.InsertAsync(annotation);
    }

    public async Task DeleteAsync(Guid annotationId, long deletedBy)
    {
        await _repository.HardDeleteAsync(annotationId);
    }

    public async Task<List<DmsAnnotation>> ListAsync(Guid documentId, Guid? versionId = null)
    {
        var query = (await _repository.GetAllAsync())
            .Where(a => a.DocumentId == documentId);

        if (versionId.HasValue)
        {
            query = query.Where(a => a.VersionId == versionId.Value);
        }

        return await query.OrderBy(a => a.PageNumber).ToListAsync();
    }

    public async Task<List<DmsAnnotation>> ListRedactionsAsync(Guid documentId)
    {
        return await (await _repository.GetAllAsync())
            .Where(a => a.DocumentId == documentId && a.IsRedaction)
            .ToListAsync();
    }
}
