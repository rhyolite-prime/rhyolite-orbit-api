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

namespace RhyoliteOrbit.DomainServices.Dms.Folders;

public class FolderManager: DomainService, IFolderManager
{
    private readonly IRepository<DmsFolder, Guid> _folderRepository;
    private readonly IRepository<DmsCollection, Guid> _collectionRepository;
    private readonly IRepository<DmsDocument, Guid> _documentRepository;
    private readonly IAbpSession _abpSession;

    public FolderManager(
        IRepository<DmsFolder, Guid> folderRepository,
        IRepository<DmsCollection, Guid> collectionRepository,
        IRepository<DmsDocument, Guid> documentRepository,
        IAbpSession abpSession)
    {
        _folderRepository = folderRepository;
        _collectionRepository = collectionRepository;
        _documentRepository = documentRepository;
        _abpSession = abpSession;
    }

    public async Task<DmsFolder> CreateAsync(DmsFolder folder)
    {
        var tenantId = _abpSession.TenantId ?? 0;

        if (folder.ParentId.HasValue)
        {
            var parent = await GetAsync(folder.ParentId.Value);
            if (parent == null)
            {
                throw new UserFriendlyException("DmsFolderNotFound", "The parent folder was not found.");
            }

            folder.CollectionId = parent.CollectionId;
            folder.Depth = parent.Depth + 1;
        }
        else
        {
            folder.Depth = 0;
        }

        await EnsureUniqueNameAsync(folder.Name, folder.ParentId, null);

        folder.Path = await BuildPathAsync(folder.ParentId, folder.Name);
        folder.TenantId = tenantId;

        return await _folderRepository.InsertAsync(folder);
    }

    public async Task<DmsFolder> UpdateAsync(DmsFolder folder)
    {
        var existing = await GetAsync(folder.Id);
        if (existing == null)
        {
            throw new UserFriendlyException("DmsFolderNotFound", "The requested folder was not found.");
        }

        if (!string.Equals(existing.Name, folder.Name, StringComparison.OrdinalIgnoreCase))
        {
            await EnsureUniqueNameAsync(folder.Name, existing.ParentId, existing.Id);
            existing.Path = await BuildPathAsync(existing.ParentId, folder.Name);
        }

        existing.Name = folder.Name;
        existing.Description = folder.Description;
        existing.TemplateId = folder.TemplateId;
        existing.IdentifierId = folder.IdentifierId;
        existing.InheritPermissions = folder.InheritPermissions;

        return await _folderRepository.UpdateAsync(existing);
    }

    public async Task MoveAsync(Guid folderId, Guid? newParentId, long movedBy)
    {
        var folder = await GetAsync(folderId);
        if (folder == null)
        {
            throw new UserFriendlyException("DmsFolderNotFound", "The requested folder was not found.");
        }

        if (folder.ParentId == newParentId)
        {
            return;
        }

        if (newParentId.HasValue)
        {
            if (newParentId.Value == folderId)
            {
                throw new UserFriendlyException("A folder cannot be moved into itself.");
            }

            var parent = await GetAsync(newParentId.Value);
            if (parent == null)
            {
                throw new UserFriendlyException("DmsFolderNotFound", "The target folder was not found.");
            }

            if (await IsDescendantAsync(parent, folderId))
            {
                throw new UserFriendlyException("A folder cannot be moved into one of its own subfolders.");
            }

            folder.Depth = parent.Depth + 1;
        }
        else
        {
            folder.Depth = 0;
        }

        folder.ParentId = newParentId;
        await _folderRepository.UpdateAsync(folder);

        await RebuildPathsAsync();
    }

    public async Task<DmsFolder> CopyAsync(Guid folderId, Guid? targetParentId, long copiedBy)
    {
        var source = await GetAsync(folderId);
        if (source == null)
        {
            throw new UserFriendlyException("DmsFolderNotFound", "The requested folder was not found.");
        }

        var copy = new DmsFolder
        {
            Name = await SuggestCopyNameAsync(source.Name, targetParentId),
            Description = source.Description,
            ParentId = targetParentId ?? source.ParentId,
            CollectionId = source.CollectionId,
            TemplateId = source.TemplateId,
            IdentifierId = source.IdentifierId,
            InheritPermissions = source.InheritPermissions,
            OwnerId = copiedBy
        };

        var created = await CreateAsync(copy);
        return created;
    }

    public async Task DeleteAsync(Guid folderId, long deletedBy)
    {
        var folder = await GetAsync(folderId);
        if (folder == null)
        {
            throw new UserFriendlyException("DmsFolderNotFound", "The requested folder was not found.");
        }

        await SoftDeleteRecursiveAsync(folder);
    }

    public async Task RestoreAsync(Guid folderId, long restoredBy)
    {
        var folder = await _folderRepository.FirstOrDefaultAsync(folderId);

        if (folder == null)
        {
            throw new UserFriendlyException("DmsFolderNotFound", "The requested folder was not found.");
        }

        folder.IsDeleted = false;
        folder.DeleterUserId = null;
        folder.DeletionTime = null;

        await _folderRepository.UpdateAsync(folder);
    }

    public async Task<List<DmsFolder>> GetChildrenAsync(Guid? parentId, bool includeDeleted = false)
    {
        var query = await _folderRepository.GetAllAsync();

        query = parentId.HasValue
            ? query.Where(f => f.ParentId == parentId.Value)
            : query.Where(f => f.ParentId == null || f.ParentId == Guid.Empty);

        if (!includeDeleted)
        {
            query = query.Where(f => !f.IsDeleted);
        }

        return await query.OrderBy(f => f.Name).ToListAsync();
    }

    public async Task<List<DmsFolder>> GetTreeAsync()
    {
        var all = await (await _folderRepository.GetAllAsync())
            .Where(f => !f.IsDeleted)
            .OrderBy(f => f.Path)
            .ToListAsync();

        return all;
    }

    public async Task<DmsFolder> GetAsync(Guid folderId)
    {
        return await _folderRepository.FirstOrDefaultAsync(folderId);
    }

    public async Task<DmsFolder> GetRootFolderAsync()
    {
        var children = await GetChildrenAsync(null);

        if (children.Any())
        {
            return children.First();
        }

        var defaultCollection = await GetOrCreateDefaultCollectionAsync();

        var root = new DmsFolder
        {
            Name = "Documents",
            Description = "Repository root",
            ParentId = null,
            CollectionId = defaultCollection.Id,
            Depth = 0,
            InheritPermissions = true
        };

        root.Path = await BuildPathAsync(null, root.Name);

        return await _folderRepository.InsertAsync(root);
    }

    public async Task<int> CountDocumentsAsync(Guid folderId)
    {
        return await (await _documentRepository.GetAllAsync())
            .CountAsync(d => d.FolderId == folderId && !d.IsDeleted);
    }

    public async Task RebuildPathsAsync()
    {
        var all = await (await _folderRepository.GetAllAsync()).ToListAsync();

        foreach (var folder in all.OrderBy(f => f.Depth))
        {
            var parent = folder.ParentId.HasValue
                ? all.FirstOrDefault(f => f.Id == folder.ParentId.Value)
                : null;

            var parentPath = parent?.Path ?? string.Empty;
            var path = string.IsNullOrEmpty(parentPath) ? folder.Name : $"{parentPath}/{folder.Name}";

            if (path != folder.Path)
            {
                folder.Path = path;
                await _folderRepository.UpdateAsync(folder);
            }
        }
    }

    private async Task<string> BuildPathAsync(Guid? parentId, string name)
    {
        if (!parentId.HasValue)
        {
            return name;
        }

        var parent = await GetAsync(parentId.Value);
        var parentPath = parent?.Path ?? string.Empty;

        return string.IsNullOrEmpty(parentPath) ? name : $"{parentPath}/{name}";
    }

    private async Task EnsureUniqueNameAsync(string name, Guid? parentId, Guid? excludeFolderId)
    {
        var query = await _folderRepository.GetAllAsync();
        query = parentId.HasValue
            ? query.Where(f => f.ParentId == parentId.Value)
            : query.Where(f => f.ParentId == null || f.ParentId == Guid.Empty);

        if (excludeFolderId.HasValue)
        {
            query = query.Where(f => f.Id != excludeFolderId.Value);
        }

        var exists = await query.AnyAsync(f => f.Name.ToLower() == name.Trim().ToLower());

        if (exists)
        {
            throw new UserFriendlyException("A folder with the same name already exists in this location.");
        }
    }

    private async Task<string> SuggestCopyNameAsync(string name, Guid? parentId)
    {
        var query = await _folderRepository.GetAllAsync();
        query = parentId.HasValue
            ? query.Where(f => f.ParentId == parentId.Value)
            : query.Where(f => f.ParentId == null || f.ParentId == Guid.Empty);

        var existing = await query.Select(f => f.Name).ToListAsync();

        var candidate = $"{name} (copy)";
        var counter = 1;

        while (existing.Any(n => string.Equals(n, candidate, StringComparison.OrdinalIgnoreCase)))
        {
            counter++;
            candidate = $"{name} (copy {counter})";
        }

        return candidate;
    }

    private async Task<bool> IsDescendantAsync(DmsFolder ancestor, Guid folderId)
    {
        var children = await (await _folderRepository.GetAllAsync())
            .Where(f => f.ParentId == ancestor.Id)
            .ToListAsync();

        foreach (var child in children)
        {
            if (child.Id == folderId)
            {
                return true;
            }

            if (await IsDescendantAsync(child, folderId))
            {
                return true;
            }
        }

        return false;
    }

    private async Task SoftDeleteRecursiveAsync(DmsFolder folder)
    {
        var documents = await (await _documentRepository.GetAllAsync())
            .Where(d => d.FolderId == folder.Id && !d.IsDeleted)
            .ToListAsync();

        foreach (var document in documents)
        {
            document.IsDeleted = true;
            document.DeletionTime = DateTime.UtcNow;
            await _documentRepository.UpdateAsync(document);
        }

        var children = await (await _folderRepository.GetAllAsync())
            .Where(f => f.ParentId == folder.Id && !f.IsDeleted)
            .ToListAsync();

        foreach (var child in children)
        {
            await SoftDeleteRecursiveAsync(child);
        }

        folder.IsDeleted = true;
        folder.DeletionTime = DateTime.UtcNow;
        await _folderRepository.UpdateAsync(folder);
    }

    public async Task<DmsCollection> GetOrCreateDefaultCollectionAsync()
    {
        var tenantId = _abpSession.TenantId ?? 0;

        var existing = await (await _collectionRepository.GetAllAsync())
            .FirstOrDefaultAsync(c => c.IsDefault);

        if (existing != null)
        {
            return existing;
        }

        var collection = new DmsCollection
        {
            Name = "Default",
            Description = "Default document repository",
            IsDefault = true,
            TenantId = tenantId,
            OwnerId = _abpSession.UserId ?? 0
        };

        return await _collectionRepository.InsertAsync(collection);
    }
}
