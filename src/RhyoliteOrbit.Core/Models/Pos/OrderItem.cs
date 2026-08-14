using System;

namespace RhyoliteOrbit.Models.Pos;

public class OrderItem
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; }
    public string Sku { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal ItemWeight { get; set; }
    public string Image { get; set; }
    public string ReceiptNo { get; set; }
    public bool HasDelivery { get; set; }
}