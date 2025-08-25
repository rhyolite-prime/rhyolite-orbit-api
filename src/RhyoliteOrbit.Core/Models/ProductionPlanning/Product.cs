using System;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class Product : Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public string Name { get; set; }
    public string Category { get; set; }
    public string UnitOfMeasure { get; set; }
    public decimal StandardCost { get; set; }
    public decimal SellingPrice { get; set; }
    public int TenantId { get; set; }
}