using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.SchoolManager;

public class BillPayment: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid AcademicYearId { get; set; }
    [MaxLength(50)]
    public string AcademicYearName { get; set; }
    public Guid TermId { get; set; }
    [MaxLength(50)]
    public string TermName { get; set; }
    public Guid ClassId { get; set; }
    [MaxLength(50)]
    public string ClassName { get; set; }
    public Guid StudentId { get; set; }
    [MaxLength(20)]
    public string StudentIdentifier { get; set; }
    [MaxLength(1200)]
    public string StudentName { get; set; }
    public decimal AmountPaid { get; set; }
    [MaxLength(20)]
    public string ModeOfPayment { get; set; }
    public DateTime PaymentDate { get; set; }
    public Guid CurrencyId { get; set; }
    [MaxLength(50)]
    public string CurrencyName { get; set; }
    [MaxLength(20)]
    public string CurrencyCode { get; set; }
    public decimal CurrencyBuyRate { get; set; }
    public decimal CurrencySellRate { get; set; }
    [MaxLength(50)]
    public string ChequeNo { get; set; }
    public Guid BillId { get; set; }
    [MaxLength(50)]
    public string BillNo { get; set; }
    [MaxLength(50)]
    public string ReceiptNo { get; set; }
    [MaxLength(100)]
    public string PaymentDescription { get; set; }
    public bool IsCreditMemo { get; set; }
    public bool IsPosted { get; set; }
    public int TenantId { get; set; }
}