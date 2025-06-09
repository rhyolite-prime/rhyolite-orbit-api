using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.SchoolManager;

public class PastQuestion: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
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
    [MaxLength(100)]
    public string Caption { get; set; }
    [MaxLength(350)]
    public string FileUrl { get; set; }
    public int TenantId { get; set; }
}