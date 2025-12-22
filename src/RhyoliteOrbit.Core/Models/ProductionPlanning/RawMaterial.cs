using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class RawMaterial: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(20)]
    public string Sku { get; set; }   

    [MaxLength(150)]
    public string Name { get; set; }   

    [MaxLength(250)]
    public string Description { get; set; }

    [MaxLength(100)]
    public string UnitOfMeasure { get; set; }
    
    public Guid UnitOfMeasureId { get; set; }

    public decimal CurrentStock { get; set; }
    
    [MaxLength(50)]
    public string CostingMethod { get; set; }
    public decimal StandardCost { get; set; }
    
    public decimal SellingPrice { get; set; }
    
    public int InventoryCategory { get; set; } //InventoryCategory enum
    
    public bool IsActive { get; set; }
    public int TenantId { get; set; }
    
}