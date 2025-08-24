using System;

public class LessonPlan : Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public string Title { get; set; }
    public string Subject { get; set; }
    public string GradeLevel { get; set; }
    public string Content { get; set; }
    public DateTime PlanDate { get; set; }
    public int TenantId { get; set; }
}