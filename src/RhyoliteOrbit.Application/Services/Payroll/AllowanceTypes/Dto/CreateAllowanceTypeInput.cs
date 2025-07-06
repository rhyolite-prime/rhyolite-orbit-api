using System;
using System.Collections.Generic;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.Services.Payroll.AllowanceTypes.Dto;

public class CreateAllowanceTypeInput
{
    public string Name { get; set; }
    public int AllowanceDays { get; set; }
    public Guid ExpenseAccountId { get; set; }
    public string ExpenseAccountName { get; set; }
    public bool Taxable { get; set; }
    public List<AllowanceRate> AllowanceRates { get; set; }
}

public class UpdateAllowanceTypeInput
{
    public string Name { get; set; }
    public int AllowanceDays { get; set; }
    public Guid ExpenseAccountId { get; set; }
    public string ExpenseAccountName { get; set; }
    public bool Taxable { get; set; }
    public List<AllowanceRate> AllowanceRates { get; set; }
}