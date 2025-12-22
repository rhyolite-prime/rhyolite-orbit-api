using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.Shared.Currencies.Dto;

public class CreateCurrencyInput
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string MinorName { get; set; }
    public decimal BuyRate { get; set; }
    public decimal SellRate { get; set; }
    public bool IsDefault { get; set; }
}

public class UpdateCurrencyInput: EntityDto<Guid>
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string MinorName { get; set; }
    public decimal BuyRate { get; set; }
    public decimal SellRate { get; set; }
    public bool IsDefault { get; set; }
}