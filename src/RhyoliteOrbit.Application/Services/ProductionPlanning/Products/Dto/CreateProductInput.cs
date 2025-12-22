using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.ProductionPlanning.Products.Dto;

public class CreateProductInput
{
    public string Name { get; set; }
    public string Category { get; set; }
    public string UnitOfMeasure { get; set; }
    public decimal StandardCost { get; set; }
    public decimal SellingPrice { get; set; }
}

public class UpdateProductInput: EntityDto<Guid>
{
    public string Name { get; set; }
    public string Category { get; set; }
    public string UnitOfMeasure { get; set; }
    public decimal StandardCost { get; set; }
    public decimal SellingPrice { get; set; }
}