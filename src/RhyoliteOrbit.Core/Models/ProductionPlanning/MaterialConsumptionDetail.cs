using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class MaterialConsumptionDetail
{
    public Guid MaterialConsumptionId { get; set; }

    public Guid MaterialId { get; set; }

    [MaxLength(100)]
    public string MaterialName { get; set; }

    public decimal QuantityUsed { get; set; }

    [MaxLength(20)]
    public string UnitOfMeasure { get; set; }

    public decimal? UnitCost { get; set; }

    public decimal? TotalCost { get; set; }

    [MaxLength(50)]
    public string BatchNumber { get; set; }

    [MaxLength(50)]
    public string StorageLocation { get; set; }
 
}