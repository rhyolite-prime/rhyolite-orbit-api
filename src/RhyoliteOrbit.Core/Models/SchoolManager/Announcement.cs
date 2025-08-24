using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class Announcement: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public string Title { get; set; }
    public string Message { get; set; }
    public string Audience { get; set; } // e.g., "Students", "Teachers", "All"
    public bool IsActive { get; set; }
    public int TenantId { get; set; }
}