using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.Services.Payroll.BikTypes.Dto;

public class CreateBikTypeInput
{
    public string Name { get; set; }
    public Guid ExpenseAccountId { get; set; }
    public bool IsTaxable { get; set; }
}

public class UpdateBikTypeInput: EntityDto<Guid>
{
    public string Name { get; set; }
    public Guid ExpenseAccountId { get; set; }
    public bool IsTaxable { get; set; }
}