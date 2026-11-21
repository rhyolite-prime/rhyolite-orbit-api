using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Dms;

namespace RhyoliteOrbit.DomainServices.Dms.Acl;

/// <summary>
/// Evaluates and maintains access control lists on collections, folders and documents.
/// Deny entries win over allow entries; entries on the document win over inherited
/// entries from the folder chain.
/// </summary>
public interface IAclManager
{
    Task<DmsAclEntry> GrantAsync(
        RhyoliteOrbitConsts.SecurableType securableType,
        Guid securableId,
        RhyoliteOrbitConsts.AclSubjectType subjectType,
        long subjectId,
        RhyoliteOrbitConsts.DmsPermission permission,
        RhyoliteOrbitConsts.AclEffect effect,
        long grantedBy);

    Task RevokeAsync(Guid entryId, long revokedBy);

    Task<List<DmsAclEntry>> GetEntriesAsync(RhyoliteOrbitConsts.SecurableType securableType, Guid securableId);

    /// <summary>
    /// Computes the effective permission bitmask for a user on a securable object.
    /// The owner of the object always receives full control.
    /// </summary>
    Task<RhyoliteOrbitConsts.DmsPermission> GetEffectivePermissionsAsync(
        RhyoliteOrbitConsts.SecurableType securableType,
        Guid securableId,
        long userId,
        IReadOnlyList<long> roleIds);

    Task EnsurePermissionAsync(
        RhyoliteOrbitConsts.SecurableType securableType,
        Guid securableId,
        long userId,
        IReadOnlyList<long> roleIds,
        RhyoliteOrbitConsts.DmsPermission permission);

    /// <summary>Copies the ACL of a folder onto a copy of it (used by folder copy).</summary>
    Task CopyEntriesAsync(
        RhyoliteOrbitConsts.SecurableType securableType,
        Guid sourceSecurableId,
        Guid targetSecurableId,
        long copiedBy);
}
