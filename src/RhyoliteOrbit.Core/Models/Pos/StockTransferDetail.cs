using System;

namespace RhyoliteOrbit.Models.Pos;

public class StockTransferDetail
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string Sku { get; set; }
    public string ItemName { get; set; }
    public string UnitOfMeasure { get; set; } // e.g., "Pcs", "Box", "Kg"
    public int QuantityTransferred { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost => QuantityTransferred * UnitCost;
    public string BatchNumber { get; set; }
}


public class StockReceiptDetail
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }

    public string Sku { get; set; }

    public string ItemName { get; set; }

    public string UnitOfMeasure { get; set; } // e.g., "Pcs", "Kg", "Box"

    public int QuantityTransferred { get; set; } // Quantity shipped from source

    public int QuantityReceived { get; set; } // Quantity actually accepted at destination

    public int Variance => QuantityReceived - QuantityTransferred; // Discrepancy (negative = missing)
    public decimal UnitCost { get; set; }

    public decimal TotalCost => QuantityTransferred * UnitCost; // Or QuantityReceived * UnitCost depending on business rule
    
    public string BatchNumber { get; set; }

    public string Remarks { get; set; } // Reason for variance (e.g., "2 items damaged in transit")
}