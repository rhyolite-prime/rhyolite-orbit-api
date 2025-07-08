using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.Services.Payroll.BikTypes.Dto;

public class CreateBikTypeInput
{
    public string Name { get; set; }
    public Guid ExpenseAccountId { get; set; }
    public bool Taxable { get; set; }
    public List<BikRate> BikRates { get; set; }
}

public class UpdateBikTypeInput
{
    public string Name { get; set; }
    public Guid ExpenseAccountId { get; set; }
    public bool Taxable { get; set; }
     public List<BikRate> BikRates { get; set; }
}