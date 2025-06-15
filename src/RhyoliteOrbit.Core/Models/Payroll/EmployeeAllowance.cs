using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Payroll;

public class EmployeeAllowance: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid EmployeeId { get; set; }
    [MaxLength(20)]
    public string EmployeeIdentifier { get; set; }
    [MaxLength(130)]
    public string EmployeeName { get; set; }
    public Guid AllowanceTypeId { get; set; }
    [MaxLength(100)]
    public string AllowanceTypeName { get; set; }
    public decimal Amount { get; set; }
    public bool Taxable { get; set; }
    public bool IsMonthly { get; set; }
    public bool Ssf { get; set; }
    public bool ProvidentFund { get; set; }
    public int AllowanceDays { get; set; }
    public int TenantId { get; set; }
}