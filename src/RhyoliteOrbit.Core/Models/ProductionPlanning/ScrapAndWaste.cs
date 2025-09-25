using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class ScrapAndWaste: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(20)]
    public string ProductionOrderIdentifier { get; set; }
    
    public Guid ProductionOrderId { get; set; }

    public int ScrapType { get; set; }   // ScrapType enum

    public decimal Quantity { get; set; }
    
    [MaxLength(100)]
    public string UnitOfMeasure { get; set; }
    public Guid UnitOfMeasureId { get; set; }

    [MaxLength(250)]
    public string Reason { get; set; }
    
    public int TenantId { get; set; }
    
}