using System;

namespace RhyoliteOrbit.Models.Dms;

public class DmsDocumentMetadataValues: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid DocumentId { get; set; }
    public Guid AttributeId { get; set; }
    public string Value { get; set; }
    public int TenantId { get; set; }
}