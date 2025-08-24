using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class Club : Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime? FoundedDate { get; set; }
    public int TenantId { get; set; }
}