using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.SchoolManager;

public class SportsTeam: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string CoachOrManager { get; set; }
    public string Category { get; set; } // e.g., Football, Basketball
    public DateTime FoundedDate { get; set; }
    public string Nickname { get; set; } // "The Eagles", "Lightning Bolts"
    public string LogoUrl { get; set; }
    [Column(TypeName = "jsonb")] public List<string> AssistantCoaches { get; set; } // Or separate entity
    public Guid CaptainId { get; set; } // Team captain
    public string CaptainName { get; set; } 
    public string CaptainClass { get; set; } 
    public string AgeGroup { get; set; } // "Under-18", "Senior", "Youth"
    public int Status { get; set; }
    //stats
    public int GamesPlayed { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
    public int Points { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int CurrentRanking { get; set; }
    public int TenantId { get; set; }
}