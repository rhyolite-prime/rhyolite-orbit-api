using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.Payroll.EmployeeOnetimeDeductions.Dto;

public class CreateEmployeeOnetimeDeductionInput
{
    public Guid EmployeeId { get; set; }
    public string EmployeeIdentifier { get; set; }
    public string EmployeeName { get; set; }
    public Guid DeductionTypeId { get; set; }
    public string DeductionTypeName { get; set; }
    public bool IsFixedAmount { get; set; }
    public decimal Percentage { get; set; }
    public decimal Amount { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}

public class UpdateEmployeeOnetimeDeductionInput: EntityDto<Guid>
{
    public Guid EmployeeId { get; set; }
    public string EmployeeIdentifier { get; set; }
    public string EmployeeName { get; set; }
    public Guid DeductionTypeId { get; set; }
    public string DeductionTypeName { get; set; }
    public bool IsFixedAmount { get; set; }
    public decimal Percentage { get; set; }
    public decimal Amount { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}