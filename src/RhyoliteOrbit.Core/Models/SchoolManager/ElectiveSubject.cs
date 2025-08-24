using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class ElectiveSubject: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public string SubjectName { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public string Level { get; set; }
    public int TenantId { get; set; }
}