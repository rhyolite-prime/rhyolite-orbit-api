using System;

namespace RhyoliteOrbit.Models.Payroll;

public class AllowanceRate
{
    public Guid AllowanceTypeId { get; set; }
    public Guid Id { get; set; }
    public string AllowanceTypeName { get; set; }
    public Guid EmployeeCategoryId { get; set; }
    public string EmployeeCategoryName { get; set; }
    public bool FixedAmount { get; set; }
    public decimal PercentageBasic { get; set; }
    public decimal Amount { get; set; }
    public decimal MaximumAmount { get; set; }
    public bool Prorate { get; set; }
    public bool ApplyBackPay { get; set; }
    public bool Taxable { get; set; }
    public int TenantId { get; set; }
}