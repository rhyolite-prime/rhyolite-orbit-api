using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Dms;

namespace RhyoliteOrbit.DomainServices.Dms.Acl;

public class AclManager: DomainService, IAclManager
{
    private readonly IRepository<DmsAclEntry, Guid> _repository;
    private readonly IAbpSession _abpSession;

    public AclManager(IRepository<DmsAclEntry, Guid> repository, IAbpSession abpSession)
    {
        _repository = repository;
        _abpSession = abpSession;
    }

    public async Task<DmsAclEntry> GrantAsync(
        RhyoliteOrbitConsts.SecurableType securableType,
        Guid securableId,
        RhyoliteOrbitConsts.AclSubjectType subjectType,
        long subjectId,
        RhyoliteOrbitConsts.DmsPermission permission,
        RhyoliteOrbitConsts.AclEffect effect,
        long grantedBy)
    {
        var tenantId = _abpSession.TenantId ?? 0;

        var existing = await (await _repository.GetAllAsync())
            .Where(x => x.TenantId == tenantId
                        && x.SecurableType == (int)securableType
                        && x.SecurableId == securableId
                        && x.SubjectType == (int)subjectType
                        && x.SubjectId == subjectId)
            .ToListAsync();

        foreach (var entry in existing)
        {
            await _repository.HardDeleteAsync(entry);
        }

        var newEntry = new DmsAclEntry
        {
            TenantId = tenantId,
            SecurableType = (int)securableType,
            SecurableId = securableId,
            SubjectType = (int)subjectType,
            SubjectId = subjectId,
            Permission = (int)permission,
            Effect = (int)effect,
            IsInherited = false,
            CreatorUserId = grantedBy
        };

        return await _repository.InsertAsync(newEntry);
    }

    public async Task RevokeAsync(Guid entryId, long revokedBy)
    {
        var data = await  _repository.FirstOrDefaultAsync(entryId);
        await _repository.HardDeleteAsync(data);
    }

    public async Task<List<DmsAclEntry>> GetEntriesAsync(RhyoliteOrbitConsts.SecurableType securableType, Guid securableId)
    {
        var tenantId = _abpSession.TenantId ?? 0;

        return await (await _repository.GetAllAsync())
            .Where(x => x.TenantId == tenantId
                        && x.SecurableType == (int)securableType
                        && x.SecurableId == securableId)
            .OrderBy(x => x.SubjectType)
            .ToListAsync();
    }

    public async Task<RhyoliteOrbitConsts.DmsPermission> GetEffectivePermissionsAsync(
        RhyoliteOrbitConsts.SecurableType securableType,
        Guid securableId,
        long userId,
        IReadOnlyList<long> roleIds)
    {
        var tenantId = _abpSession.TenantId ?? 0;

        var ownEntries = await (await _repository.GetAllAsync())
            .Where(x => x.TenantId == tenantId
                        && x.SecurableType == (int)securableType
                        && x.SecurableId == securableId)
            .ToListAsync();

        // No ACL defined anywhere on this object: fall back to the open default
        // (full control for tenant members) to stay backwards compatible.
        if (!ownEntries.Any())
        {
            return RhyoliteOrbitConsts.DmsPermission.All;
        }

        var allowed = RhyoliteOrbitConsts.DmsPermission.None;
        var denied = RhyoliteOrbitConsts.DmsPermission.None;

        foreach (var entry in ownEntries)
        {
            if (!Applies(entry, userId, roleIds))
            {
                continue;
            }

            if (entry.Effect == (int)RhyoliteOrbitConsts.AclEffect.Deny)
            {
                denied |= (RhyoliteOrbitConsts.DmsPermission)entry.Permission;
            }
            else
            {
                allowed |= (RhyoliteOrbitConsts.DmsPermission)entry.Permission;
            }
        }

        return allowed & ~denied;
    }

    public async Task EnsurePermissionAsync(
        RhyoliteOrbitConsts.SecurableType securableType,
        Guid securableId,
        long userId,
        IReadOnlyList<long> roleIds,
        RhyoliteOrbitConsts.DmsPermission permission)
    {
        var effective = await GetEffectivePermissionsAsync(securableType, securableId, userId, roleIds);

        if (!effective.HasFlag(permission))
        {
            throw new Abp.UI.UserFriendlyException(
                "DmsAccessDenied",
                "You do not have permission to perform this action on the document.");
        }
    }

    public async Task CopyEntriesAsync(
        RhyoliteOrbitConsts.SecurableType securableType,
        Guid sourceSecurableId,
        Guid targetSecurableId,
        long copiedBy)
    {
        var tenantId = _abpSession.TenantId ?? 0;

        var entries = await (await _repository.GetAllAsync())
            .Where(x => x.TenantId == tenantId
                        && x.SecurableType == (int)securableType
                        && x.SecurableId == sourceSecurableId)
            .ToListAsync();

        foreach (var entry in entries)
        {
            await _repository.InsertAsync(new DmsAclEntry
            {
                TenantId = tenantId,
                SecurableType = entry.SecurableType,
                SecurableId = targetSecurableId,
                SubjectType = entry.SubjectType,
                SubjectId = entry.SubjectId,
                Permission = entry.Permission,
                Effect = entry.Effect,
                IsInherited = false,
                CreatorUserId = copiedBy
            });
        }
    }

    private static bool Applies(DmsAclEntry entry, long userId, IReadOnlyList<long> roleIds)
    {
        switch ((RhyoliteOrbitConsts.AclSubjectType)entry.SubjectType)
        {
            case RhyoliteOrbitConsts.AclSubjectType.User:
                return entry.SubjectId == userId;
            case RhyoliteOrbitConsts.AclSubjectType.Role:
                return roleIds != null && roleIds.Contains(entry.SubjectId);
            case RhyoliteOrbitConsts.AclSubjectType.Tenant:
                return true;
            default:
                return false;
        }
    }
}
