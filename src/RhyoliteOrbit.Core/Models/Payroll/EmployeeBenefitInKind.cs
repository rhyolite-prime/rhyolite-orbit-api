using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Payroll;

public class EmployeeBenefitInKind: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid EmployeeId { get; set; }
    [MaxLength(20)]
    public string EmployeeIdentifier { get; set; }
    [MaxLength(130)]
    public string EmployeeName { get; set; }
    public Guid BenefitInKindTypeId { get; set; }
    [MaxLength(100)]
    public string BenefitInKindTypeName { get; set; }
    public decimal Amount { get; set; }
    public int TenantId { get; set; }
}