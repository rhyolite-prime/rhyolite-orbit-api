using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.DocumentManagement;

namespace RhyoliteOrbit.DomainServices.DocumentManagement.Documents;

public class DocumentManager : Abp.Domain.Services.DomainService, IDocumentManager
{
    private readonly IRepository<Document, Guid> _repositoryDocument;

    public DocumentManager(IRepository<Document, Guid> repositoryDocument)
    {
        _repositoryDocument = repositoryDocument;
    }

    public async Task Create(Document entity)
    {
        NormalizeAndValidate(entity);

        var existingDocument = await _repositoryDocument.FirstOrDefaultAsync(x =>
            x.TenantId == entity.TenantId && !x.IsDeleted && x.Title == entity.Title);

        if (existingDocument == null)
        {
            await _repositoryDocument.InsertAsync(entity);
            return;
        }

        throw new UserFriendlyException(
            400,
            "Duplicate records are not allowed.",
            "A Document with the same title already exists."
        );
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<Document>();

        predicate.And(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalizedQuery = query.Trim().ToLower();
            predicate.And(x => x.Title.ToLower().Contains(normalizedQuery));
        }

        var totalCount = await _repositoryDocument.CountAsync(predicate);

        var data = await (await _repositoryDocument.GetAllAsync())
            .Where(predicate)
            .Select(a => new
            {
                a.Id,
                a.Title,
                a.Description,
                a.FileUrl,
                a.FileName,
                a.FileType,
                a.FileSize,
                a.Category,
                a.CreationTime
            })
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalPages = totalCount > 0 ? Convert.ToInt32(Math.Ceiling(totalCount / (decimal)pageSize)) : 0;

        return new
        {
            pageNo,
            pageSize,
            totalCount,
            totalPages,
            data,
            lowerBound = totalCount == 0 ? 0 : pageSize * (pageNo - 1) + 1,
            upperBound = totalCount == 0 ? 0 : totalPages == pageNo ? totalCount : pageNo * pageSize
        };
    }

    public async Task Update(Document entity)
    {
        NormalizeAndValidate(entity);

        var existingDocument = await _repositoryDocument.FirstOrDefaultAsync(x =>
            x.TenantId == entity.TenantId && !x.IsDeleted && x.Title == entity.Title && x.Id != entity.Id);

        if (existingDocument != null)
        {
            throw new UserFriendlyException(
                400,
                "Duplicate records are not allowed.",
                "A Document with the same title already exists."
            );
        }

        await _repositoryDocument.UpdateAsync(entity);
    }

    public async Task Delete(Guid id)
    {
        await _repositoryDocument.DeleteAsync(id);
    }

    private static void NormalizeAndValidate(Document entity)
    {
        entity.Title = entity.Title?.Trim();
        entity.Description = entity.Description?.Trim();
        entity.FileUrl = entity.FileUrl?.Trim();
        entity.FileName = entity.FileName?.Trim();
        entity.FileType = entity.FileType?.Trim();
        entity.Category = entity.Category?.Trim();

        if (string.IsNullOrWhiteSpace(entity.Title))
        {
            throw new UserFriendlyException(400, "Title is required.");
        }

        if (string.IsNullOrWhiteSpace(entity.FileUrl))
        {
            throw new UserFriendlyException(400, "FileUrl is required.");
        }

        if (entity.FileSize < 0)
        {
            throw new UserFriendlyException(400, "FileSize cannot be negative.");
        }
    }
}
