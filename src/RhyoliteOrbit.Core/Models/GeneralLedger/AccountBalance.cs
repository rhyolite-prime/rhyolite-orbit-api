using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.GeneralLedger;

public class AccountBalance: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid AccountId { get; set; }
    [MaxLength(150)]
    public string AccountName { get; set; }
    public decimal CreditBalance { get; set; }
    public decimal DebitBalance { get; set; }
    public decimal CreditForeign { get; set; }
    public decimal DebitForeign { get; set; }
    public Guid OuId { get; set; }
    [MaxLength(200)]
    public string OuName { get; set; }
    public Guid CurrencyId { get; set; }
    [MaxLength(100)]
    public string CurrencyName { get; set; }
    [MaxLength(20)]
    public string CurrencyCode { get; set; }
    public decimal BuyRate { get; set; }
    public decimal SellRate { get; set; }
    public int TenantId { get; set; }
}