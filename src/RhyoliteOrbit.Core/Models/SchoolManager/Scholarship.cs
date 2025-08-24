using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class Scholarship: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; }
    public string StudentClass { get; set; }
    public string ScholarshipType { get; set; } // e.g., Merit-based, Need-based
    public DateTime AwardDate { get; set; }
    public decimal Amount { get; set; }
    public string Sponsor { get; set; }
    public string Remarks { get; set; }
    public int TenantId { get; set; }
}