using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.SchoolManager;

public class ResultsUpload: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid AcademicYearId { get; set; }
    [MaxLength(50)]
    public string AcademicYearName { get; set; }
    public Guid ClassId { get; set; }
    [MaxLength(50)]
    public string ClassName { get; set; }
    public Guid SubjectId { get; set; }
    [MaxLength(50)]
    public string SubjectName { get; set; }
    public Guid TermId { get; set; }
    [MaxLength(50)]
    public string TermName { get; set; }
    public Guid ResultTypeId { get; set; }
    [MaxLength(50)]
    public string ResultTypeName { get; set; }
    public decimal TotalMarks { get; set; }
    public Guid StudentId { get; set; }
    [MaxLength(20)]
    public string StudentIdentifier { get; set; }
    [MaxLength(130)]
    public string StudentName { get; set; }
    public decimal MarksObtained { get; set; }
    public int TenantId { get; set; }
}