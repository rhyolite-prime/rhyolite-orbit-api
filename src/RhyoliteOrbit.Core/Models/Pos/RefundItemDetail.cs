using System;

namespace RhyoliteOrbit.Models.Pos;

public class RefundItemDetail
{
    public Guid ItemId { get; set; }

    public string Sku { get; set; }

    public string ItemName { get; set; }

    public int QuantityReturned { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice => QuantityReturned * UnitPrice;

    public string Condition { get; set; } // e.g., "Unopened", "Damaged", "Defective"
}