using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.SchoolManager;

public class MiscellaneousPayment: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid StudentId { get; set; }
    [MaxLength(130)]
    public string StudentName { get; set; }
    public Guid ClassId { get; set; }
    [MaxLength(50)]
    public string ClassName { get; set; }
    public Guid ParentId { get; set; }
    [MaxLength(130)]
    public string ParentName { get; set; }
    public Guid PaymentTypeId { get; set; }
    [MaxLength(50)]
    public string PaymentTypeName { get; set; }
    public decimal Amount { get; set; }
    [MaxLength(50)]
    public string PaymentMethod { get; set; }
    public bool IsPosted { get; set; }
    public Guid RevenueLedgerAccountId { get; set; }
    public Guid OuId { get; set; }
    [MaxLength(50)]
    public string OuName { get; set; }
    [MaxLength(20)]
    public string ReceiptNo { get; set; }
    public Guid CurrencyId { get; set; }
    [MaxLength(50)]
    public string CurrencyName { get; set; }
    [MaxLength(20)]
    public string CurrencyCode { get; set; }
    public int TenantId { get; set; }
}