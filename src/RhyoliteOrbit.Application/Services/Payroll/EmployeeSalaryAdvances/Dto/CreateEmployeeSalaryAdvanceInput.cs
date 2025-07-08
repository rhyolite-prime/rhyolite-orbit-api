using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.Payroll.EmployeeSalaryAdvances.Dto;

public class CreateEmployeeSalaryAdvanceInput
{
    public Guid EmployeeId { get; set; }
    public string EmployeeIdentifier { get; set; }
    public string EmployeeName { get; set; }
    public DateTime LoanDate { get; set; }
    public Guid LoanTypeId { get; set; }
    public string LoanTypeName { get; set; }
    public decimal Amount { get; set; }
    public bool IsApproved { get; set; }
    public string Status { get; set; }
    public bool IsPaid { get; set; }
}

public class UpdateEmployeeSalaryAdvanceInput : EntityDto<Guid>
{
    public Guid EmployeeId { get; set; }
    public string EmployeeIdentifier { get; set; }
    public string EmployeeName { get; set; }
    public DateTime LoanDate { get; set; }
    public Guid LoanTypeId { get; set; }
    public string LoanTypeName { get; set; }
    public decimal Amount { get; set; }
    public bool IsApproved { get; set; }
    public string Status { get; set; }
    public bool IsPaid { get; set; }
}