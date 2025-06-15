using System;

namespace RhyoliteOrbit.Models.Payroll;

public class InitializePayMonth: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public int Month { get; set; }
    public int Year { get; set; }
    public int TenantId { get; set; }
}