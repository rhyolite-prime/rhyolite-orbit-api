using System.Collections.Generic;

namespace RhyoliteOrbit.Models.Shared;

public class SubscriptionPricingOptions
{
    public decimal UsdExchangeRate { get; set; } = 11m;
    public List<ModulePriceOptions> Modules { get; set; } = new();
    
    public BillingCycleMultipliers BillingCycleMultipliers { get; set; } = new();

}

public sealed class ModulePriceOptions
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal PriceInUsd { get; set; }
}

public sealed class BillingCycleMultipliers
{
    public decimal Quarterly { get; set; } = 1m;
    public decimal HalfYearly { get; set; } = 1.9m;
    public decimal Annual { get; set; } = 3.4m;

}
