using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Pos;

public class CustomerRank: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(205)]
    public string Name { get; set; }
    public decimal MinimumTotalSpend { get; set; }
    public decimal InStoreDiscount { get; set; }
    public decimal OnlineDiscount { get; set; }
    public Guid CurrencyId { get; set; }
    [MaxLength(20)]
    public string CurrencyCode { get; set; }
    public int TenantId { get; set; }
}