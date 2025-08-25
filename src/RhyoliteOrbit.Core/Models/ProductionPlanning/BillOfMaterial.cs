using System;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class BillOfMaterial: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public string Name { get; set; }
    public string Type { get; set; } = "Raw"; // Raw, Semi-finished, Consumable
    public string UnitOfMeasure { get; set; }
    public decimal CurrentStock { get; set; }
    public decimal ReorderLevel { get; set; }
    public int TenantId { get; set; }
}