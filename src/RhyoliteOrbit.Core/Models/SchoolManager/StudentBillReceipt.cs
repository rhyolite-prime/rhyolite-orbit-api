using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.SchoolManager;

public class StudentBillReceipt: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
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
    [MaxLength(130)]
    public string ReceivedFrom { get; set; }
    public decimal AmountPaid { get; set; }
    [MaxLength(120)]
    public string AmountInWords { get; set; }
    [MaxLength(100)]
    public string Reason { get; set; }
    public decimal BalanceDue { get; set; }
    [MaxLength(50)]
    public string PaymentType { get; set; }
    [MaxLength(20)]
    public string ReceiptNo { get; set; }
    public Guid StudentId { get; set; }
    [MaxLength(20)]
    public string StudentIdentifier { get; set; }
    [MaxLength(130)]
    public string StudentName { get; set; }
    public Guid PaymentId { get; set; }
    public int TenantId { get; set; }

}