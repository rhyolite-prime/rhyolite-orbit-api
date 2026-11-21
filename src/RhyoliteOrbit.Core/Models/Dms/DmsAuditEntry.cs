using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>
/// Append only audit record. Entries form a SHA-256 hash chain per tenant so tampering
/// can be detected (see <c>HashChainAuditTrail</c>).
/// </summary>
public class DmsAuditEntry: Abp.Domain.Entities.Auditing.CreationAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid? DocumentId { get; set; }

    public Guid? FolderId { get; set; }

    /// <summary>Value of <see cref="RhyoliteOrbitConsts.AuditAction"/>.</summary>
    public int Action { get; set; }

    public long? ActorId { get; set; }

    [Column(TypeName = "jsonb")]
    public string DetailsJson { get; set; }

    [MaxLength(100)]
    public string IpAddress { get; set; }

    [MaxLength(300)]
    public string UserAgent { get; set; }

    public long Sequence { get; set; }

    [MaxLength(64)]
    public string PreviousHash { get; set; }

    [MaxLength(64)]
    public string EntryHash { get; set; }

    public int TenantId { get; set; }
}
