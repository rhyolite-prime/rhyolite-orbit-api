using System;

namespace RhyoliteOrbit.Models.Pos;

public class ProductAttribute
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public decimal PriceAdjustment { get; set; }
    public bool PriceAdjustmentAsPercentage { get; set; }
}