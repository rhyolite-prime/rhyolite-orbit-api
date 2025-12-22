using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class BillOfMaterialItem : Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid ProductId { get; set; }
    [MaxLength(100)]
    public string ProductName { get; set; }
    [MaxLength(250)]
    public string Description { get; set; }
    public Guid MaterialId { get; set; }
    [MaxLength(100)]
    public string MaterialName { get; set; }
    public decimal QuantityPerUnit { get; set; }
    [MaxLength(100)]
    public string UnitOfMeasure { get; set; }
    
    public int TenantId { get; set; }
}