using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class StudentAward: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; }
    public string StudentClass { get; set; }
    public string AwardName { get; set; }
    public DateTime AwardDate { get; set; }
    public string Description { get; set; }
    public string Issuer { get; set; }
    public int TenantId { get; set; }
}