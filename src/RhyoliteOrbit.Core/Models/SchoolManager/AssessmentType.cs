using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class AssessmentType : Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int TenantId { get; set; }
}