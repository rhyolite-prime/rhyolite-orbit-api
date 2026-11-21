using System;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>Cached storage accounting for a tenant, refreshed by a scheduled job.</summary>
public class DmsQuotaUsage: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public long BytesUsed { get; set; }

    public int DocumentCount { get; set; }

    public int VersionCount { get; set; }

    public DateTime ComputedAt { get; set; }

    public int TenantId { get; set; }
}
