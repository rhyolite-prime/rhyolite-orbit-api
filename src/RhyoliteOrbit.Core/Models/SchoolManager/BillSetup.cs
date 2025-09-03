using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.SchoolManager;

public class BillSetup: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid AcademicYearId { get; set; }
    [MaxLength(50)]
    public string AcademicYearName { get; set; }
    public Guid ClassId { get; set; }
    [MaxLength(50)]
    public string ClassName { get; set; }
    public Guid TermId { get; set; }
    [MaxLength(50)]
    public string TermName { get; set; }
    public Guid BillTypeId { get; set; }
    [MaxLength(50)]
    public string BillTypeName { get; set; }
    public decimal TotalBillAmount { get; set; }
    [Column(TypeName = "jsonb")] public List<BillSetupDetail> Details { get; set; }
    public int TenantId { get; set; }
}