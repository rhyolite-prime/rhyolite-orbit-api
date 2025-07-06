using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.Payroll.TaxTables.Dto;

public class CreateTaxTableInput
{
    public decimal Rate { get; set; }
    public decimal UpperLimitOfAmount { get; set; }
}

public class UpdateTaxTableInput: EntityDto<Guid>
{
    public decimal Rate { get; set; }
    public decimal UpperLimitOfAmount { get; set; }
}