using System;

namespace RhyoliteOrbit.Models.Dms;

public class DmsDocumentTag: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid DocumentId { get; set; }
    public Guid TagId { get; set; }
    public int TenantId { get; set; }
}