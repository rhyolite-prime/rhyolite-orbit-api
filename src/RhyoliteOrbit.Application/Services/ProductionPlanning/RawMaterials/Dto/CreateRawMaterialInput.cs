using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.ProductionPlanning.RawMaterials.Dto;

public class CreateRawMaterialInput
{
    public string Sku { get; set; }   

    public string Name { get; set; }   

    public string Description { get; set; }

    public Guid UnitOfMeasureId { get; set; }
    
    public string UnitOfMeasure { get; set; }

    public decimal CurrentStock { get; set; }
     
    public string CostingMethod { get; set; }
     
    public decimal StandardCost { get; set; }

    public int InventoryCategory { get; set; } = (int)RhyoliteOrbitConsts.InventoryCategory.RawMaterial;
    
    public bool IsActive { get; set; }
}

public class UpdateRawMaterialInput: EntityDto<Guid>
{
    public string Sku { get; set; }   

    public string Name { get; set; }   

    public string Description { get; set; }

    public Guid UnitOfMeasureId { get; set; }
    
    public string UnitOfMeasure { get; set; }

    public decimal CurrentStock { get; set; }
     
    public string CostingMethod { get; set; }
     
    public decimal StandardCost { get; set; }
    
    public int InventoryCategory { get; set; } = (int)RhyoliteOrbitConsts.InventoryCategory.RawMaterial;
    
    public bool IsActive { get; set; }
}