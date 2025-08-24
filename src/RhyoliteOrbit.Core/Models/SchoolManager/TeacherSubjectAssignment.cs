using System;

public class TeacherSubjectAssignment : Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid TeacherId { get; set; }
    public string TeacherName { get; set; }
    public Guid SubjectId { get; set; }
    public string SubjectName { get; set; }
    public Guid AcademicYearId { get; set; }
    public string AcademicYearName { get; set; }
    public Guid ClassId { get; set; }
    public string ClassName { get; set; }
    public int TenantId { get; set; }
}