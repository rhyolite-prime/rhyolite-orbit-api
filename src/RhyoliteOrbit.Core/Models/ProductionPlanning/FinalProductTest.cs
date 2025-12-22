using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class FinalProductTest: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid ProductionOrderId { get; set; }
    [MaxLength(20)]
    public string ProductionOrderIdentifier { get; set; }
    public DateTime TestDate { get; set; }

    public Guid InspectorId { get; set; }
    [MaxLength(130)]
    public string InspectorName { get; set; }

    public Guid QcChecklistId { get; set; }
    
    [MaxLength(100)]
    public string QcChecklistName { get; set; }
    
    [Column(TypeName = "jsonb")] public List<InspectionResult> Results { get; set; }

    public int Status { get; set; } // FinalProductTestStatus enum
    
    public int TenantId { get; set; }
}