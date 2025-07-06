using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.Payroll.EmployeeAllowances.Dto;

public class CreateEmployeeAllowanceInput
{
    public Guid EmployeeId { get; set; }
    public string EmployeeIdentifier { get; set; }
    public string EmployeeName { get; set; }
    public Guid AllowanceTypeId { get; set; }
    public string AllowanceTypeName { get; set; }
    public decimal Amount { get; set; }
    public bool IsTaxable { get; set; }
    public bool IsMonthly { get; set; }
    public bool Ssf { get; set; }
    public bool ProvidentFund { get; set; }
    public int AllowanceDays { get; set; }
}

public class UpdateEmployeeAllowanceInput: EntityDto<Guid>
{
    public Guid EmployeeId { get; set; }
    public string EmployeeIdentifier { get; set; }
    public string EmployeeName { get; set; }
    public Guid AllowanceTypeId { get; set; }
    public string AllowanceTypeName { get; set; }
    public decimal Amount { get; set; }
    public bool IsTaxable { get; set; }
    public bool IsMonthly { get; set; }
    public bool Ssf { get; set; }
    public bool ProvidentFund { get; set; }
    public int AllowanceDays { get; set; }
    public int TenantId { get; set; }
}

public class ImportEmployeeAllowanceInput
{
    public string EmployeeIdentifier { get; set; }
    public string AllowanceTypeName { get; set; }
    public decimal Amount { get; set; }
    public bool Taxable { get; set; }
    public int AllowanceDays { get; set; }
}