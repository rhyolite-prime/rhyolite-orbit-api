using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.GeneralLedger;

public class CoaDetail: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(150)]
    public string AccountName { get; set; }
    public int AccountNo { get; set; }
    public int Status { get; set; }
    public Guid AccountHeaderId { get; set; }
    [MaxLength(100)]
    public string AccountHeaderName { get; set; }
    public Guid CurrencyId { get; set; }
    [MaxLength(50)]
    public string CurrencyName { get; set; }
    [MaxLength(20)]
    public string CurrencyCode { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal CurrentForeignBalance { get; set; }
    public int TenantId { get; set; }
}