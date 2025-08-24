using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class ExamSchedule : Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public string Title { get; set; }
    public string Subject { get; set; }
    public string GradeLevel { get; set; }
    public DateTime ExamDate { get; set; }
    public string Venue { get; set; }
    public int TenantId { get; set; }
}