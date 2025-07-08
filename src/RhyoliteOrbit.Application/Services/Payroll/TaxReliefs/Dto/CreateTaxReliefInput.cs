using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.Payroll.TaxReliefs.Dto;

public class CreateTaxReliefInput
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
}

public class UpdateTaxReliefInput: EntityDto<Guid>
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
}