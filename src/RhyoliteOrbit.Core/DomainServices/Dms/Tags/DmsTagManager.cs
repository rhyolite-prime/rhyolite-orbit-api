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

namespace RhyoliteOrbit.DomainServices.Dms.Tags;

public interface IDmsTagManager
{
    Task<DmsTag> CreateAsync(DmsTag tag);

    Task<DmsTag> UpdateAsync(DmsTag tag);

    Task DeleteAsync(Guid tagId);

    Task<List<DmsTag>> ListAsync(string query);

    Task<object> GetTagCloudAsync(int maxTags);

    Task AssignAsync(Guid documentId, List<Guid> tagIds);

    Task<List<DmsTag>> GetDocumentTagsAsync(Guid documentId);
}

public class DmsTagManager: DomainService, IDmsTagManager
{
    private readonly IRepository<DmsTag, Guid> _tagRepository;
    private readonly IRepository<DmsDocumentTag, Guid> _documentTagRepository;
    private readonly IAbpSession _abpSession;

    public DmsTagManager(
        IRepository<DmsTag, Guid> tagRepository,
        IRepository<DmsDocumentTag, Guid> documentTagRepository,
        IAbpSession abpSession)
    {
        _tagRepository = tagRepository;
        _documentTagRepository = documentTagRepository;
        _abpSession = abpSession;
    }

    public async Task<DmsTag> CreateAsync(DmsTag tag)
    {
        var tenantId = _abpSession.TenantId ?? 0;

        var exists = await (await _tagRepository.GetAllAsync())
            .AnyAsync(t => t.Name.ToLower() == tag.Name.Trim().ToLower());

        if (exists)
        {
            throw new UserFriendlyException("Duplicate records are not allowed.", "A duplicate tag with the same name already exists.");
        }

        tag.TenantId = tenantId;

        return await _tagRepository.InsertAsync(tag);
    }

    public async Task<DmsTag> UpdateAsync(DmsTag tag)
    {
        var existing = await _tagRepository.FirstOrDefaultAsync(tag.Id);
        if (existing == null)
        {
            throw new UserFriendlyException("The tag was not found.");
        }

        existing.Name = tag.Name;
        existing.ColorCode = tag.ColorCode;
        existing.Description = tag.Description;

        return await _tagRepository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(Guid tagId)
    {
        var links = await (await _documentTagRepository.GetAllAsync())
            .Where(t => t.TagId == tagId)
            .ToListAsync();

        foreach (var link in links)
        {
            await _documentTagRepository.HardDeleteAsync(link);
        }

        await _tagRepository.HardDeleteAsync(tagId);
    }

    public async Task<List<DmsTag>> ListAsync(string query)
    {
        var tags = await _tagRepository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim().ToLower();
            tags = tags.Where(t => t.Name.ToLower().Contains(term));
        }

        return await tags.OrderBy(t => t.Name).ToListAsync();
    }

    public async Task<object> GetTagCloudAsync(int maxTags)
    {
        if (maxTags <= 0)
        {
            maxTags = 50;
        }

        var links = await (await _documentTagRepository.GetAllAsync())
            .GroupBy(t => t.TagId)
            .Select(g => new { TagId = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(maxTags)
            .ToListAsync();

        var tagIds = links.Select(l => l.TagId).ToList();

        var tags = await (await _tagRepository.GetAllAsync())
            .Where(t => tagIds.Contains(t.Id))
            .ToListAsync();

        return links.Select(l => new
        {
            l.TagId,
            l.Count,
            Name = tags.FirstOrDefault(t => t.Id == l.TagId)?.Name,
            ColorCode = tags.FirstOrDefault(t => t.Id == l.TagId)?.ColorCode
        });
    }

    public async Task AssignAsync(Guid documentId, List<Guid> tagIds)
    {
        var existing = await (await _documentTagRepository.GetAllAsync())
            .Where(t => t.DocumentId == documentId)
            .ToListAsync();

        foreach (var link in existing)
        {
            await _documentTagRepository.HardDeleteAsync(link);
        }

        foreach (var tagId in tagIds ?? new List<Guid>())
        {
            await _documentTagRepository.InsertAsync(new DmsDocumentTag
            {
                TenantId = _abpSession.TenantId ?? 0,
                DocumentId = documentId,
                TagId = tagId
            });
        }
    }

    public async Task<List<DmsTag>> GetDocumentTagsAsync(Guid documentId)
    {
        var tagIds = await (await _documentTagRepository.GetAllAsync())
            .Where(t => t.DocumentId == documentId)
            .Select(t => t.TagId)
            .ToListAsync();

        return await (await _tagRepository.GetAllAsync())
            .Where(t => tagIds.Contains(t.Id))
            .ToListAsync();
    }
}
