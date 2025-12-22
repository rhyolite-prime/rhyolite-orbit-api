using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class MaterialConsumption: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid ProductionOrderId { get; set; }
    
    [MaxLength(20)]
    public string ProductionOrderIdentifier { get; set; }

    [MaxLength(20)]
    public string ConsumptionIdentifier { get; set; }  // e.g. MCN-000123

    public Guid WorkCenterId { get; set; }
    
    [MaxLength(150)]
    public string WorkCenterName { get; set; }

    [Column(TypeName = "jsonb")] public List<MaterialConsumptionDetail> Details { get; set; }
    
    public int TenantId { get; set; }
    
}