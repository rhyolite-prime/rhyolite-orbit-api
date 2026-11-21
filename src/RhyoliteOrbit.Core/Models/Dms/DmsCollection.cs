using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>
/// Top level repository (a "cabinet") that owns a folder tree. Every tenant has one
/// default collection; additional collections can model departments or projects.
/// </summary>
public class DmsCollection: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(150)]
    public string Description { get; set; }

    public int OwnerId { get; set; }

    public bool IsDefault { get; set; }

    /// <summary>Optional storage ceiling in bytes; 0 means "use the tenant quota".</summary>
    public long StorageQuotaBytes { get; set; }

    public int TenantId { get; set; }
}
