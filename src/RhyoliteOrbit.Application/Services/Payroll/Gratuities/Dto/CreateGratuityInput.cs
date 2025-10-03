using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.Payroll.Gratuities.Dto;

public class CreateGratuityInput
{
    public decimal MinYears { get; set; }
    public decimal MaxYears { get; set; }
    public decimal Factor { get; set; }
}

public class UpdateGratuityInput: EntityDto<Guid>
{
    public decimal MinYears { get; set; }
    public decimal MaxYears { get; set; }
    public decimal Factor { get; set; }
}