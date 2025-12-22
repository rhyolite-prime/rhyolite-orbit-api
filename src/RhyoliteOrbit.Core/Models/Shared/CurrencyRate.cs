using System;

namespace RhyoliteOrbit.Models.Shared;

public class CurrencyRate
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal SellingRate { get; set; }
    public decimal BuyingRate { get; set; }
}