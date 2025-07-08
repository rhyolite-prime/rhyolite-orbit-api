using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Payroll;

public class MonthlyAllowance: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
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
    public Guid AllowanceTypeId { get; set; }
    [MaxLength(100)]
    public string AllowanceTypeName { get; set; }
    public decimal Amount { get; set; }
    public bool IsTaxable { get; set; }
    public bool Ssf { get; set; }
    public bool ProvidentFund { get; set; }
    public int AllowanceDays { get; set; }
    public bool IsMonthly { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public Guid CurrencyId { get; set; }
    [MaxLength(100)]
    public string CurrencyName { get; set; }
    public decimal BuyRate { get; set; }
    public decimal SellRate { get; set; }
    public int TenantId { get; set; }
}