using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.Services.Payroll.AllowanceTypes.Dto;

public class CreateAllowanceTypeInput
{
    public string Name { get; set; }
    public int AllowanceDays { get; set; }
    public Guid ExpenseAccountId { get; set; }
    public string ExpenseAccountName { get; set; }
    public bool IsTaxable { get; set; }
}

public class UpdateAllowanceTypeInput: EntityDto<Guid>
{
    public string Name { get; set; }
    public int AllowanceDays { get; set; }
    public Guid ExpenseAccountId { get; set; }
    public string ExpenseAccountName { get; set; }
    public bool IsTaxable { get; set; }
}