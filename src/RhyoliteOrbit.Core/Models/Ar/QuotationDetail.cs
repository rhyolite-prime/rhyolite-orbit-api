using System;

namespace RhyoliteOrbit.Models.Ar;

public class QuotationDetail
{
    public Guid Id { get; set; }
    public string ReferenceNo { get; set; }
    public string Description { get; set; }
    public Guid DebitAccountId { get; set; }
    public string DebitAccountName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
    public bool DiscountAsPercentage { get; set; }
    public decimal Discount { get; set; }
    public decimal SubTotal { get; set; }
}

public class ArQuotationTaxLevies
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public decimal Rate { get; set; }
    public bool IsApplied { get; set; }
}