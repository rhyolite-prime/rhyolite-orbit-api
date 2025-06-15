using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.SchoolManager;

public class TerminalReport: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid AcademicYearId { get; set; }
    public Guid ClassId { get; set; }
    public Guid TermId { get; set; }
    public Guid StudentId { get; set; }
    [MaxLength(130)]
    public string StudentName { get; set; }
    [MaxLength(20)]
    public string StudentIdentifier { get; set; }
    [MaxLength(50)]
    public string ClassName { get; set; }
    [MaxLength(50)]
    public string AcademicYearName { get; set; }
    [MaxLength(50)]
    public string PromotedTo { get; set; }
    public decimal TermAverage { get; set; }
    public int NumberOnRoll { get; set; }
    [MaxLength(50)]
    public string TermName { get; set; }
    [MaxLength(50)]
    public string NextTermBegins { get; set; }
    public int Position { get; set; }
    public int Attendance { get; set; }
    public int TotalAttendanceDays { get; set; }
    [MaxLength(130)]
    public string ClassTeacher { get; set; }
    [MaxLength(150)]
    public string ClassTeacherRemarks { get; set; }
    public string HeadTeacherSignatureImageUrl { get; set; }
    [Column(TypeName = "jsonb")] public List<SubjectResult> SubjectResults { get; set; }
    public int TenantId { get; set; }
}