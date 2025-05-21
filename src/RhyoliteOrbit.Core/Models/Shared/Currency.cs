using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Shared;

public class Currency: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string CurrencyName { get; set; }
    [MaxLength(50)]
    public string CurrencyCode { get; set; }
    [MaxLength(50)]
    public string MinorName { get; set; }
    public decimal BuyRate { get; set; }
    public decimal SellRate { get; set; }
    public bool IsDefault { get; set; }
    [Column(TypeName = "jsonb")] public List<CurrencyRate> Rates { get; set; }
    public int TenantId { get; set; }
}