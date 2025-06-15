using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class StudentParent: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid StudentId { get; set; }
    public Guid ParentId { get; set; }
    public int TenantId { get; set; }
}