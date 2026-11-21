using System;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>The value of a custom attribute for a document.</summary>
public class DmsDocumentMetadataValues: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid DocumentId { get; set; }

    public Guid AttributeId { get; set; }

    public string Value { get; set; }

    public int TenantId { get; set; }
}
