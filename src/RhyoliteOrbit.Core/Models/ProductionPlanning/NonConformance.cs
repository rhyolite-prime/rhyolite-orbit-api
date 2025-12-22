using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class NonConformance : Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid InProcessInspectionId { get; set; }
    
    public Guid FinalProductTestingId { get; set; }

    [MaxLength(200)]
    public string Description { get; set; }
    public int Severity { get; set; } // SeverityLevel enum
    [MaxLength(200)]
    public string RootCause { get; set; }
    [MaxLength(200)]
    public string CorrectiveAction { get; set; }
    [MaxLength(200)]
    public string PreventiveAction { get; set; }

    public DateTime ReportedAt { get; set; }
    
    public Guid ReportedById { get; set; }
    
    [MaxLength(130)]
    public string ReportedByName { get; set; }

    public int Status { get; set; } // NonConformanceStatus enum
    public int TenantId { get; set; }
}