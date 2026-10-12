using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization.Roles;
using RhyoliteOrbit.Authorization.Users;
using RhyoliteOrbit.DomainServices.Dms.Acl;
using RhyoliteOrbit.DomainServices.Dms.Audit;
using RhyoliteOrbit.DomainServices.Dms.Documents;
using RhyoliteOrbit.DomainServices.Dms.Folders;
using RhyoliteOrbit.Models.Dms;

namespace RhyoliteOrbit.Services.DocumentManagement;

/// <summary>
/// Shared plumbing for the DMS application services: current user/tenant resolution,
/// role lookup and the effective ACL walk (document -> folder -> collection).
/// </summary>
public abstract class DmsAppServiceBase: RhyoliteOrbitAppServiceBase
{
    protected IAclManager AclManager { get; }

    protected IDocumentManager DocumentManager { get; }

    protected IFolderManager FolderManager { get; }

    protected IAuditTrail AuditTrail { get; }

    private readonly IRepository<UserRole, long> _userRoleRepository;
    private readonly IRepository<Role, int> _roleRepository;

    protected DmsAppServiceBase(
        IAclManager aclManager,
        IDocumentManager documentManager,
        IFolderManager folderManager,
        IAuditTrail auditTrail,
        IRepository<UserRole, long> userRoleRepository,
        IRepository<Role, int> roleRepository)
    {
        AclManager = aclManager;
        DocumentManager = documentManager;
        FolderManager = folderManager;
        AuditTrail = auditTrail;
        _userRoleRepository = userRoleRepository;
        _roleRepository = roleRepository;
    }

    protected long CurrentUserId
    {
        get
        {
            var userId = AbpSession.GetUserId();
            if (userId == 0)
            {
                throw new Abp.UI.UserFriendlyException("DmsNoUser", "The current user could not be determined.");
            }

            return userId;
        }
    }

    protected int CurrentTenantId => AbpSession.GetTenantId();

    protected async Task<IReadOnlyList<long>> GetRoleIdsAsync()
    {
        var userId = CurrentUserId;

        var roleIds = await (await _userRoleRepository.GetAllAsync())
            .Where(x => x.UserId == userId)
            .Select(x => x.RoleId)
            .ToListAsync();

        if (roleIds.Count == 0)
        {
            return Array.Empty<long>();
        }

        // Static/tenant roles may have been hard deleted: keep only existing ones.
        var existing = await (await _roleRepository.GetAllAsync())
            .Where(x => roleIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync();

        return existing.Select(x => (long)x).ToList();
    }

    protected async Task<bool> HasEntriesAsync(RhyoliteOrbitConsts.SecurableType type, Guid securableId)
    {
        var entries = await AclManager.GetEntriesAsync(type, securableId);
        return entries.Count > 0;
    }

    /// <summary>
    /// Walks the ACL chain document -> folder -> collection and throws when the
    /// current user does not hold the requested permission. Objects without any
    /// ACL entry inherit from their parent; when nothing is defined at all the
    /// tenant default (full control for members) applies.
    /// </summary>
    protected async Task EnsureDocumentPermissionAsync(Guid documentId, RhyoliteOrbitConsts.DmsPermission permission)
    {
        var userId = CurrentUserId;
        var roleIds = await GetRoleIdsAsync();

        var document = await DocumentManager.GetAsync(documentId);

        if (await HasEntriesAsync(RhyoliteOrbitConsts.SecurableType.Document, documentId))
        {
            await AclManager.EnsurePermissionAsync(
                RhyoliteOrbitConsts.SecurableType.Document, documentId, userId, roleIds, permission);
            return;
        }

        if (await HasEntriesAsync(RhyoliteOrbitConsts.SecurableType.Folder, document.FolderId))
        {
            await AclManager.EnsurePermissionAsync(
                RhyoliteOrbitConsts.SecurableType.Folder, document.FolderId, userId, roleIds, permission);
            return;
        }

        if (await HasEntriesAsync(RhyoliteOrbitConsts.SecurableType.Collection, document.CollectionId))
        {
            await AclManager.EnsurePermissionAsync(
                RhyoliteOrbitConsts.SecurableType.Collection, document.CollectionId, userId, roleIds, permission);
        }
    }

    protected async Task EnsureFolderPermissionAsync(Guid folderId, RhyoliteOrbitConsts.DmsPermission permission)
    {
        var userId = CurrentUserId;
        var roleIds = await GetRoleIdsAsync();

        var folder = await FolderManager.GetAsync(folderId);

        if (await HasEntriesAsync(RhyoliteOrbitConsts.SecurableType.Folder, folderId))
        {
            await AclManager.EnsurePermissionAsync(
                RhyoliteOrbitConsts.SecurableType.Folder, folderId, userId, roleIds, permission);
            return;
        }

        if (await HasEntriesAsync(RhyoliteOrbitConsts.SecurableType.Collection, folder.CollectionId))
        {
            await AclManager.EnsurePermissionAsync(
                RhyoliteOrbitConsts.SecurableType.Collection, folder.CollectionId, userId, roleIds, permission);
        }
    }

    /// <summary>Builds a downloadable response for content produced by the pipeline.</summary>
    protected FileStreamResult File(Stream content, string contentType, string fileName)
    {
        return new FileStreamResult(content, contentType) { FileDownloadName = fileName };
    }
}
