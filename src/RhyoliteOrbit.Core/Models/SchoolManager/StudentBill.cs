using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.SchoolManager;

public class StudentBill: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid ClassId { get; set; }
    [MaxLength(50)]
    public string ClassName { get; set; }
    public Guid AcademicYearId { get; set; }
    [MaxLength(50)]
    public string AcademicYearName { get; set; }
    public Guid TermId { get; set; }
    [MaxLength(50)]
    public string TermName { get; set; }
    [MaxLength(50)]
    public string BillNo { get; set; }
    public DateTime BillDate { get; set; }
    [MaxLength(50)]
    public string CurrencyName { get; set; }
    [MaxLength(20)]
    public string CurrencyCode { get; set; }
    [MaxLength(30)]
    public string MinorName { get; set; }
    public decimal BuyRate { get; set; }
    public decimal SellRate { get; set; }
    public decimal BillAmount { get; set; }
    public decimal BillBalance { get; set; }
    public Guid StudentId { get; set; }
    [MaxLength(20)]
    public string StudentIdentifier { get; set; }
    [MaxLength(130)]
    public string StudentName { get; set; }
    public int BillStatus { get; set; } 
    [MaxLength(100)]
    public string Description { get; set; }
    public Guid BillTypeId { get; set; }
    [MaxLength(50)]
    public string BillTypeName { get; set; }  
    public Guid BillSetupId { get; set; }
    [Column(TypeName = "jsonb")] public BillSetupInfo BillSetupInfo { get; set; }  
    [Column(TypeName = "jsonb")] public List<BillDetail> Details { get; set; }  
    public int TenantId { get; set; }
    
}