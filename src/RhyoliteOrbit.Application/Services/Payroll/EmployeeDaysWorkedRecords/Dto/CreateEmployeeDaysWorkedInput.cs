using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.Payroll.EmployeeDaysWorkedRecords.Dto;

public class CreateEmployeeDaysWorkedInput
{
    public Guid EmployeeId { get; set; }
    public string EmployeeIdentifier { get; set; }
    public string EmployeeName { get; set; }
    public int Days { get; set; }
    public decimal Hours { get; set; }
    public decimal Minutes { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}

public class UpdateEmployeeDaysWorkedInput: EntityDto<Guid>
{
    public Guid EmployeeId { get; set; }
    public string EmployeeIdentifier { get; set; }
    public string EmployeeName { get; set; }
    public int Days { get; set; }
    public decimal Hours { get; set; }
    public decimal Minutes { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}