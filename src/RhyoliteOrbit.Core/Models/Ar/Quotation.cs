using System;
using System.Collections.Generic;

namespace RhyoliteOrbit.Models.Ar;

public class Quotation: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string Address { get; set; }
    public string AccountNumber { get; set; }
    public Guid CurrencyId { get; set; }
    public string CurrencyName { get; set; }
    public string CurrencyCode { get; set; }
    public decimal BuyRate { get; set; }
    public string QuotationNumber { get; set; }
    public string Notes { get; set; }
    public bool IsConvertedToInvoice { get; set; }
    public DateTime QuotationDate { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "jsonb")]
    public List<QuotationDetail> Details { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "jsonb")]
    public List<ArQuotationTaxLevies> TaxLevies { get; set; }
    public int TenantId { get; set; }
}