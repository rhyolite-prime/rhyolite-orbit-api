using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Payroll;

public class TaxRelief:Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public int TenantId { get; set; }
}