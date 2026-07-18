using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.GeneralLedger;

public class JournalDetail: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid JournalId { get; set; }
    public Guid AccountId { get; set; }
    [MaxLength(150)]
    public string AccountName { get; set; }
    public Guid OuId { get; set; } 
    [MaxLength(350)]
    public string OuName { get; set; }
    [MaxLength(50)]
    public string ReferenceNumber { get; set; }
    [MaxLength(50)]
    public string ChequeNo { get; set; }
    [MaxLength(350)]
    public string Description { get; set; }
    public decimal LocalDebit { get; set; }
    public decimal LocalCredit { get; set; }
    public decimal ForeignDebit { get; set; }
    public decimal ForeignCredit { get; set; }
    public DateTime TransactionDate { get; set; } //can use system date
    [MaxLength(50)]
    public string TransactionSource { get; set; }
    public bool IsLocal { get; set; }
    public bool IsReconciled { get; set; }
    public bool IsOpeningBalance { get; set; }
    public bool IsConsolidated { get; set; }
    public DateTime DateConsolidated { get; set; }
    public Guid CurrencyId { get; set; }
    [MaxLength(100)]
    public string CurrencyName { get; set; }
    [MaxLength(20)]
    public string CurrencyCode { get; set; }
    public decimal BuyRate { get; set; }
    public decimal SellRate { get; set; }
    public DateTime? ClosedDate { get; set; }
    public Guid PettyCashRecipientId { get; set; }
    [MaxLength(130)]
    public string PettyCashRecipientName { get; set; }
    public int TenantId { get; set; }
}