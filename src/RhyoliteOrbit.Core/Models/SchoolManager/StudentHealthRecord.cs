using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class StudentHealthRecord: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; }
    public string StudentClass { get; set; }
    public DateTime RecordDate { get; set; }
    public string HealthIssue { get; set; }
    public string Treatment { get; set; }
    public string Notes { get; set; }
    public int TenantId { get; set; }
}