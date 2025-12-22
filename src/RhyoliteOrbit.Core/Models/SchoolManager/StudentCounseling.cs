using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class StudentCounseling: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; }
    public string StudentClass { get; set; }
    public DateTime CounselingDate { get; set; }
    public string CounselorName { get; set; }
    public string Topic { get; set; }
    public string Notes { get; set; }
    public int TenantId { get; set; }
}