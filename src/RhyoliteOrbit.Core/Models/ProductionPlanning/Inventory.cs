using System;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class Inventory: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid ProductOrMaterialId { get; set; }
    public string ProductOrMaterialName{ get; set; }
    public string Location { get; set; }
    public decimal Quantity { get; set; }
    public string BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int TenantId { get; set; }
}