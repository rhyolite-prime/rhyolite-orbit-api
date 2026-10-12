using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>
/// Polymorphic link between a document and a record of another ERP module
/// (invoice, payslip, student, POS order, non conformance, ...).
/// </summary>
public class DmsEntityLink: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    /// <summary>Value of <see cref="RhyoliteOrbitConsts.DmsEntityType"/>.</summary>
    public int EntityType { get; set; }

    [MaxLength(100)]
    public string EntityId { get; set; }

    public Guid DocumentId { get; set; }

    /// <summary>0 = Attachment, 1 = Supporting, 2 = Generated, 3 = SignedCopy (see <see cref="RhyoliteOrbitConsts.DmsLinkType"/>).</summary>
    public int LinkType { get; set; }

    [MaxLength(250)]
    public string Note { get; set; }

    public long CreatedById { get; set; }

    public int TenantId { get; set; }
}
