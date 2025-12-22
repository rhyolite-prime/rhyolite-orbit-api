using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.SchoolManager;

public class StudentBillReportHistory: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid BillSetupId { get; set; }
    public Guid AcademicYearId { get; set; }
    [MaxLength(50)]
    public string AcademicYearName { get; set; }
    public Guid TermId { get; set; }
    [MaxLength(50)]
    public string TermName { get; set; }
    [MaxLength(50)]
    public string ClassName { get; set; }
    public Guid ClassId { get; set; }
    public Guid BillTypeId { get; set; }
    public DateTime BillDate { get; set; }
    [MaxLength(50)]
    public string BillNo { get; set; }
    [MaxLength(350)]
    public string DocumentUrl { get; set; }
    public int TenantId { get; set; }
}