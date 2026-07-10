using System;

namespace RhyoliteOrbit.Services.Shared.CostCenters.Dto;

public class CreateCostCenterInput
{
    public Guid ParentId { get; set; }
    public string Name { get; set; }
    
}

public class UpdateCostCenterInput
{
    public Guid ParentId { get; set; }
    public string Name { get; set; }
}