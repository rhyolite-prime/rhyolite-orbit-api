using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class Discipline : Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; }
    public string StudentClass { get; set; }
    public DateTime IncidentDate { get; set; }
    public string Offense { get; set; }
    public string ActionTaken { get; set; }
    public string Remarks { get; set; }
    public int TenantId { get; set; }
}

