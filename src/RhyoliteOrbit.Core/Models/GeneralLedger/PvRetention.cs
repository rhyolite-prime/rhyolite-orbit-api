using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.GeneralLedger;

public class PvRetention: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Description { get; set; }
    public Guid VoucherHeaderId { get; set; }
    public Guid CreditAccountId { get; set; }
    [MaxLength(150)]
    public string CreditAccountName { get; set; }
    public bool IsFixedAmount { get; set; }
    public decimal Amount { get; set; }
    public decimal TotalAmount { get; set; }
    public int TenantId { get; set; }
}