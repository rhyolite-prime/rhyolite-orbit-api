using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class SpecialNeed: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; }
    public string StudentClass { get; set; }
    public string NeedType { get; set; } // e.g., Physical, Learning, Emotional, etc.
    public string Description { get; set; }
    public DateTime? IdentifiedDate { get; set; }
    public int TenantId { get; set; }
}