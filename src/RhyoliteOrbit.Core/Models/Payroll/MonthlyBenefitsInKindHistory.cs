using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Payroll;

public class MonthlyBenefitsInKindHistory: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid EmployeeId { get; set; }
    [MaxLength(20)]
    public string EmployeeIdentifier { get; set; }
    [MaxLength(130)]
    public string EmployeeName { get; set; }
    [MaxLength(100)]
    public string EmployeeDepartment { get; set; }
    [MaxLength(100)]
    public string EmployeeCategory { get; set; }
    public Guid BenefitInKindTypeId { get; set; }
    [MaxLength(100)]
    public string BenefitInKindTypeName { get; set; }
    public decimal Amount { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public int TenantId { get; set; }
}