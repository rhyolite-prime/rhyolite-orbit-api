using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.SchoolManager;

public class GradingScheme : Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid AcademicYearId { get; set; }
    [MaxLength(50)]
    public string AcademicYearName { get; set; }
    public Guid TermId { get; set; }
    [MaxLength(50)]
    public string TermName { get; set; }
    public Guid LevelId { get; set; }
    [MaxLength(50)]
    public string LevelName { get; set; }
    public decimal ExamPercentage { get; set; }
    public decimal ClassPercentage { get; set; }
    public int TenantId { get; set; }
}