using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.GeneralLedger;

public class Imprest: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid EmployeeId { get; set; }
    [MaxLength(20)]
    public string EmployeeIdentifier { get; set; }
    [MaxLength(130)]
    public string EmployeeName { get; set; }
    public Guid ImprestCategoryId { get; set; }
    [MaxLength(100)]
    public string ImprestCategoryName { get; set; }
    public decimal Amount { get; set; }
    public decimal Balance { get; set; }
    public DateTime DisbursementDate { get; set; }
    public DateTime ExpectedRetirementDate { get; set; }
    public Guid CreditAccountId { get; set; }
    [MaxLength(150)]
    public string CreditAccountName { get; set; }
    [MaxLength(50)]
    public string PaymentMethod { get; set; }
    [MaxLength(50)]
    public string ChequeNo { get; set; }
    [MaxLength(50)]
    public string ImprestNo { get; set; }
    public Guid DebitAccountId { get; set; }
    [MaxLength(150)]
    public string DebitAccountName { get; set; }
    [MaxLength(50)]
    public string ReferenceNo { get; set; }
    [MaxLength(350)]
    public string Description { get; set; }
    public Guid OuId { get; set; }
    [MaxLength(250)]
    public string OuName { get; set; }
    public Guid CurrencyId { get; set; }
    [MaxLength(100)]
    public string CurrencyName { get; set; }
    [MaxLength(20)]
    public string CurrencyCode { get; set; }
    public decimal BuyRate { get; set; }
    public decimal SellRate { get; set; }
    public bool PostedToGl { get; set; }
    public int TenantId { get; set; }
}