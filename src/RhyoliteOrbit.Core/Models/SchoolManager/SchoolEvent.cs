using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class SchoolEvent: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime EventDate { get; set; }
    public string Location { get; set; }
    public string Organizer { get; set; }
    public int TenantId { get; set; }
}