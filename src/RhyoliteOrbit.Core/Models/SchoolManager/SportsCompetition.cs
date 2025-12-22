using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.SchoolManager;

public class SportsCompetition: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Venue { get; set; }
    public string Organizer { get; set; }
    public string Category { get; set; } // e.g., Football, Athletics
    public decimal RegistrationFee { get; set; }
    public decimal PrizeMoney { get; set; }
    public bool DrugTestingRequired { get; set; }
    [Column(TypeName = "jsonb")] public List<string> ProhibitedItems { get; set; } // as file/asssets urls
    [Column(TypeName = "jsonb")] public List<string> RulesAndRegulations { get; set; } // as file/asssets urls
    [Column(TypeName = "jsonb")] public List<string> SafetyRequirements { get; set; } // as file/asssets urls
    [Column(TypeName = "jsonb")] public List<SportsCompetitionSchedule> Schedule { get; set; }
    public int TenantId { get; set; }
}