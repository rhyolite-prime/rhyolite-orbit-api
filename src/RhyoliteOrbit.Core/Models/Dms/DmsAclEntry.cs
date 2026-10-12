using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>
/// Access control entry attached to a collection, folder or document.
/// Permissions are evaluated deny-first, then document entries, then inherited folder entries.
/// </summary>
public class DmsAclEntry: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    /// <summary>0 = Collection, 1 = Folder, 2 = Document (see <see cref="RhyoliteOrbitConsts.SecurableType"/>).</summary>
    public int SecurableType { get; set; }

    public System.Guid SecurableId { get; set; }

    /// <summary>0 = User, 1 = Role, 2 = Department, 3 = Tenant (see <see cref="RhyoliteOrbitConsts.AclSubjectType"/>).</summary>
    public int SubjectType { get; set; }

    public long SubjectId { get; set; }

    /// <summary>Bitmask of <see cref="RhyoliteOrbitConsts.DmsPermission"/>.</summary>
    public int Permission { get; set; }

    /// <summary>0 = Allow, 1 = Deny.</summary>
    public int Effect { get; set; }

    public bool IsInherited { get; set; }

    public int TenantId { get; set; }
}
