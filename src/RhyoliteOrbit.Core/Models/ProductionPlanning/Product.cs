using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class Product : Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(100)]
    public string Category { get; set; }
    [MaxLength(100)]
    public string UnitOfMeasure { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    
    public int MinimumOrderQuantity { get; set; }
    public decimal StandardCost { get; set; }
    public decimal SellingPrice { get; set; }
    public int TenantId { get; set; }
}