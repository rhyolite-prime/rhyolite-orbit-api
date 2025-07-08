using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.Payroll.EmployeeReliefs.Dto;

public class CreateEmployeeReliefInput
{
    public Guid EmployeeId { get; set; }
    public string EmployeeIdentifier { get; set; }
    public string EmployeeName { get; set; }
    public Guid ReliefTypeId { get; set; }
    public string ReliefTypeName { get; set; }
    public decimal Amount { get; set; }
}

public class UpdateEmployeeReliefInput: EntityDto<Guid>
{
    public Guid EmployeeId { get; set; }
    public string EmployeeIdentifier { get; set; }
    public string EmployeeName { get; set; }
    public Guid ReliefTypeId { get; set; }
    public string ReliefTypeName { get; set; }
    public decimal Amount { get; set; }
}