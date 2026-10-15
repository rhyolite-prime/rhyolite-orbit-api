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

namespace RhyoliteOrbit.DomainServices.Dms.Comments;

public interface ICommentManager
{
    Task<DmsComment> CreateAsync(DmsComment comment);

    Task<DmsComment> UpdateAsync(Guid commentId, string text, long updatedBy);

    Task DeleteAsync(Guid commentId, long deletedBy);

    Task<List<DmsComment>> ListAsync(Guid documentId, Guid? versionId = null);

    Task ResolveAsync(Guid commentId, long resolvedBy);

    Task<int> CountUnresolvedAsync(Guid documentId);
}

public class CommentManager: DomainService, ICommentManager
{
    private readonly IRepository<DmsComment, Guid> _repository;
    private readonly IAbpSession _abpSession;

    public CommentManager(IRepository<DmsComment, Guid> repository, IAbpSession abpSession)
    {
        _repository = repository;
        _abpSession = abpSession;
    }

    public async Task<DmsComment> CreateAsync(DmsComment comment)
    {
        comment.TenantId = _abpSession.TenantId ?? 0;
        comment.IsEdited = false;

        if (string.IsNullOrWhiteSpace(comment.CommentType))
        {
            comment.CommentType = "General";
        }

        return await _repository.InsertAsync(comment);
    }

    public async Task<DmsComment> UpdateAsync(Guid commentId, string text, long updatedBy)
    {
        var comment = await _repository.FirstOrDefaultAsync(commentId);
        if (comment == null)
        {
            throw new UserFriendlyException("The comment was not found.");
        }

        comment.CommentText = text;
        comment.IsEdited = true;

        return await _repository.UpdateAsync(comment);
    }

    public async Task DeleteAsync(Guid commentId, long deletedBy)
    {
        var comment = await _repository.FirstOrDefaultAsync(commentId);
        await _repository.HardDeleteAsync(comment);
    }

    public async Task<List<DmsComment>> ListAsync(Guid documentId, Guid? versionId = null)
    {
        var query = (await _repository.GetAllAsync())
            .Where(c => c.DocumentId == documentId);

        if (versionId.HasValue)
        {
            query = query.Where(c => c.VersionId == versionId.Value);
        }

        return await query
            .OrderBy(c => c.CreationTime)
            .ToListAsync();
    }

    public async Task ResolveAsync(Guid commentId, long resolvedBy)
    {
        var comment = await _repository.FirstOrDefaultAsync(commentId);
        if (comment == null)
        {
            throw new UserFriendlyException("The comment was not found.");
        }

        comment.ResolvedById = resolvedBy;
        comment.ResolvedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(comment);
    }

    public async Task<int> CountUnresolvedAsync(Guid documentId)
    {
        return await (await _repository.GetAllAsync())
            .CountAsync(c => c.DocumentId == documentId && c.ResolvedAt == null);
    }
}
