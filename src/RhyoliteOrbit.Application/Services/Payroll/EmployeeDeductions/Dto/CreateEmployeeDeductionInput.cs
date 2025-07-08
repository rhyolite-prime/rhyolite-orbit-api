using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.Payroll.EmployeeDeductions.Dto;

public class CreateEmployeeDeductionInput
{
    public Guid EmployeeId { get; set; }
    public string EmployeeIdentifier { get; set; }
    public string EmployeeName { get; set; }
    public Guid DeductionTypeId { get; set; }
    public string DeductionTypeName { get; set; }
    public decimal Amount { get; set; }
    public decimal EmployerAmount { get; set; }
}


public class UpdateEmployeeDeductionInput: EntityDto<Guid>
{
    public Guid EmployeeId { get; set; }
    
    public string EmployeeIdentifier { get; set; }
    
    public string EmployeeName { get; set; }
    
    public Guid DeductionTypeId { get; set; }
    
    public string DeductionTypeName { get; set; }
    
    public decimal Amount { get; set; }
    
    public decimal EmployerAmount { get; set; }
}

public class ImportEmployeeDeductionInput
{
    public string EmployeeIdentifier { get; set; }
    public string DeductionTypeName { get; set; }
    public decimal Amount { get; set; }
    public decimal EmployerAmount { get; set; }
}