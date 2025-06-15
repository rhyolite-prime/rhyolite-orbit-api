using System;

namespace RhyoliteOrbit.Models.Payroll;

public class Gratuity: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public decimal MinYears { get; set; }
    public decimal MaxYears { get; set; }
    public decimal Factor { get; set; }
    public int TenantId { get; set; }
}