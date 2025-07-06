using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.Payroll.BoaAllowances.Dto;

public class CreateBoAllowanceInput
{
    public Guid EmployeeId { get; set; }
    [MaxLength(50)]
    public string EmployeeIdentifier { get; set; }
    [MaxLength(130)]
    public string EmployeeName { get; set; }
    public Guid AllowanceTypeId { get; set; }
    [MaxLength(100)]
    public string AllowanceTypeName { get; set; }
    public bool IsFixedAmount { get; set; }
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
    public bool IsTaxable { get; set; }
    public bool IsSsf { get; set; }
    public bool IsPf { get; set; }
    public int AllowanceDays { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public int TenantId { get; set; }
}

public class UpdateBoAllowanceInput: EntityDto<Guid>
{
    public Guid EmployeeId { get; set; }
    [MaxLength(50)]
    public string EmployeeIdentifier { get; set; }
    [MaxLength(130)]
    public string EmployeeName { get; set; }
    public Guid AllowanceTypeId { get; set; }
    [MaxLength(100)]
    public string AllowanceTypeName { get; set; }
    public bool IsFixedAmount { get; set; }
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
    public bool IsTaxable { get; set; }
    public bool IsSsf { get; set; }
    public bool IsPf { get; set; }
    public int AllowanceDays { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public int TenantId { get; set; }
}