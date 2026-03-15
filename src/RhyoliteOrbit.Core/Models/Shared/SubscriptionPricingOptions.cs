using System.Collections.Generic;

namespace RhyoliteOrbit.Models.Shared;

public class SubscriptionPricingOptions
{
    public decimal UsdExchangeRate { get; set; } = 11m;
    public List<ModulePriceOptions> Modules { get; set; } = new();

}

public sealed class ModulePriceOptions
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal PriceInUsd { get; set; }
}
