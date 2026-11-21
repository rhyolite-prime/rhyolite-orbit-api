using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.Dms.Acl;
using RhyoliteOrbit.DomainServices.Dms.Audit;
using RhyoliteOrbit.DomainServices.Dms.Folders;
using RhyoliteOrbit.Models.Dms;
using RhyoliteOrbit.Services.DocumentManagement.Dto;

namespace RhyoliteOrbit.Services.DocumentManagement.Folders;

[AbpAuthorize]
public class FolderAppService: DmsAppServiceBase
{
    private readonly IFolderManager _folders;
    private readonly IRepository<DmsCollection, Guid> _collectionRepository;
    private readonly IMapper _mapper;

    public FolderAppService(
        IFolderManager folders,
        IRepository<DmsCollection, Guid> collectionRepository,
        IAclManager aclManager,
        DomainServices.Dms.Documents.IDocumentManager documentManager,
        IAuditTrail auditTrail,
        IRepository<Authorization.Users.UserRole, long> userRoleRepository,
        IRepository<Authorization.Roles.Role, int> roleRepository,
        IMapper mapper)
        : base(aclManager, documentManager, folders, auditTrail, userRoleRepository, roleRepository)
    {
        _folders = folders;
        _collectionRepository = collectionRepository;
        _mapper = mapper;
    }

    /// <summary>Creates a folder (or a sub folder).</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageFolders)]
    [HttpPost]
    public async Task<FolderDto> Create(CreateFolderDto input)
    {
        Guid collectionId;

        if (input.ParentId.HasValue)
        {
            var parent = await _folders.GetAsync(input.ParentId.Value);
            await EnsureFolderPermissionAsync(parent.Id, RhyoliteOrbitConsts.DmsPermission.Edit);
            collectionId = parent.CollectionId;
        }
        else
        {
            collectionId = input.CollectionId ?? (await _folders.GetOrCreateDefaultCollectionAsync()).Id;
        }

        var folder = new DmsFolder
        {
            TenantId = CurrentTenantId,
            Name = input.Name,
            Description = input.Description,
            ParentId = input.ParentId,
            CollectionId = collectionId,
            OwnerId = CurrentUserId,
            TemplateId = input.TemplateId,
            IdentifierId = input.IdentifierId,
            InheritPermissions = true,
            Depth = 0,
            CreatorUserId = CurrentUserId
        };

        var created = await _folders.CreateAsync(folder);
        await _folders.RebuildPathsAsync();

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.FolderCreated, null, created.Id, null, CurrentUserId);

        return _mapper.Map<FolderDto>(created);
    }

    /// <summary>Updates a folder.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageFolders)]
    [HttpPut]
    public async Task<FolderDto> Update(UpdateFolderDto input)
    {
        var folder = await _folders.GetAsync(input.Id);
        await EnsureFolderPermissionAsync(folder.Id, RhyoliteOrbitConsts.DmsPermission.Edit);

        folder.Name = input.Name;
        folder.Description = input.Description;

        if (input.InheritPermissions.HasValue)
        {
            folder.InheritPermissions = input.InheritPermissions.Value;
        }

        var updated = await _folders.UpdateAsync(folder);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.FolderUpdated, null, updated.Id, null, CurrentUserId);

        return _mapper.Map<FolderDto>(updated);
    }

    /// <summary>Gets a folder.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<FolderDto> Get(Guid id)
    {
        var folder = await _folders.GetAsync(id);
        return _mapper.Map<FolderDto>(folder);
    }

    /// <summary>Lists the children of a folder (root when no parent is given).</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<ListResultDto<FolderDto>> GetChildren(Guid? parentId = null)
    {
        var children = await _folders.GetChildrenAsync(parentId);
        return new ListResultDto<FolderDto>(_mapper.Map<List<FolderDto>>(children));
    }

    /// <summary>Gets the complete folder tree.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<List<FolderDto>> GetTree()
    {
        var tree = await _folders.GetTreeAsync();
        return _mapper.Map<List<FolderDto>>(tree);
    }

    /// <summary>Gets (creating when needed) the root folder of the tenant.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<FolderDto> GetRoot()
    {
        var root = await _folders.GetRootFolderAsync();
        return _mapper.Map<FolderDto>(root);
    }

    /// <summary>Moves a folder to another parent.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageFolders)]
    [HttpPut]
    public async Task Move(MoveFolderDto input)
    {
        await EnsureFolderPermissionAsync(input.FolderId, RhyoliteOrbitConsts.DmsPermission.Edit);

        if (input.NewParentId.HasValue)
        {
            await EnsureFolderPermissionAsync(input.NewParentId.Value, RhyoliteOrbitConsts.DmsPermission.Edit);
        }

        await _folders.MoveAsync(input.FolderId, input.NewParentId, CurrentUserId);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.FolderMoved, null, input.FolderId, null, CurrentUserId);
    }

    /// <summary>Copies a folder (with its sub folders) to another parent.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageFolders)]
    [HttpPost]
    public async Task<FolderDto> Copy(MoveFolderDto input)
    {
        await EnsureFolderPermissionAsync(input.FolderId, RhyoliteOrbitConsts.DmsPermission.View);

        var copy = await _folders.CopyAsync(input.FolderId, input.NewParentId, CurrentUserId);
        return _mapper.Map<FolderDto>(copy);
    }

    /// <summary>Soft deletes a folder and everything below it.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageFolders)]
    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await EnsureFolderPermissionAsync(id, RhyoliteOrbitConsts.DmsPermission.Delete);
        await _folders.DeleteAsync(id, CurrentUserId);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.FolderDeleted, null, id, null, CurrentUserId);
    }

    /// <summary>Restores a folder from the trash.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageFolders)]
    [HttpPut]
    public async Task Restore(Guid id)
    {
        await EnsureFolderPermissionAsync(id, RhyoliteOrbitConsts.DmsPermission.Delete);
        await _folders.RestoreAsync(id, CurrentUserId);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.FolderRestored, null, id, null, CurrentUserId);
    }

    /// <summary>Counts the documents directly inside a folder.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<int> CountDocuments(Guid id)
    {
        return await _folders.CountDocumentsAsync(id);
    }

    /// <summary>Rebuilds the denormalised folder paths and depths.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageFolders)]
    [HttpPost]
    public async Task RebuildPaths()
    {
        await _folders.RebuildPathsAsync();
    }

    // ------------------------------------------------------------ collections

    /// <summary>Lists the document collections of the tenant.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageCollections)]
    [HttpGet]
    public async Task<ListResultDto<CollectionDto>> GetCollections()
    {
        var collections = await (await _collectionRepository.GetAllAsync())
            .Where(x => x.TenantId == CurrentTenantId)
            .OrderBy(x => x.Name)
            .ToListAsync();

        return new ListResultDto<CollectionDto>(_mapper.Map<List<CollectionDto>>(collections));
    }

    /// <summary>Creates a document collection.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageCollections)]
    [HttpPost]
    public async Task<CollectionDto> CreateCollection(CreateCollectionDto input)
    {
        var collection = new DmsCollection
        {
            TenantId = CurrentTenantId,
            Name = input.Name,
            Description = input.Description,
            OwnerId = (int)CurrentUserId,
            IsDefault = false,
            StorageQuotaBytes = input.StorageQuotaBytes,
            CreatorUserId = CurrentUserId
        };

        var created = await _collectionRepository.InsertAsync(collection);
        return _mapper.Map<CollectionDto>(created);
    }

    /// <summary>Updates a document collection.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageCollections)]
    [HttpPut]
    public async Task<CollectionDto> UpdateCollection(UpdateCollectionDto input)
    {
        var collection = await _collectionRepository.GetAsync(input.Id);

        if (collection.TenantId != CurrentTenantId)
        {
            throw new UserFriendlyException("DmsCollectionNotFound", "The requested collection was not found.");
        }

        collection.Name = input.Name;
        collection.Description = input.Description;
        collection.StorageQuotaBytes = input.StorageQuotaBytes;

        var updated = await _collectionRepository.UpdateAsync(collection);
        return _mapper.Map<CollectionDto>(updated);
    }

    /// <summary>Deletes a document collection (folders must be moved first).</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageCollections)]
    [HttpDelete]
    public async Task DeleteCollection(Guid id)
    {
        var collection = await _collectionRepository.GetAsync(id);

        if (collection.TenantId != CurrentTenantId)
        {
            throw new UserFriendlyException("DmsCollectionNotFound", "The requested collection was not found.");
        }

        if (collection.IsDefault)
        {
            throw new UserFriendlyException("DmsDefaultCollection", "The default collection cannot be deleted.");
        }

        await _collectionRepository.DeleteAsync(collection);
    }
}
