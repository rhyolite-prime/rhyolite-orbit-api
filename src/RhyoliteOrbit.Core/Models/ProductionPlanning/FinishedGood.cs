using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class FinishedGood: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(20)]
    public string BatchNumber { get; set; }
    
    [MaxLength(20)]
    public string ProductionOrderIdentifier { get; set; }
    public Guid ProductionOrderId { get; set; }
    public Guid ProductId { get; set; }
    
    [MaxLength(100)]
    public string UnitOfMeasure { get; set; }
    public Guid UnitOfMeasureId { get; set; }

    [MaxLength(100)]
    public string ProductName { get; set; }
    public decimal QuantityProduced { get; set; } // reduced during finished good allocation to customers
    
    public int InventoryCategory { get; set; } // InventoryCategory enum
    
    public DateTime ProductionDate { get; set; }
    public bool IsQaApproved { get; set; }
     
    public int TenantId { get; set; }

}