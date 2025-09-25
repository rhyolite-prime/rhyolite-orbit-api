using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using RhyoliteOrbit.Models.ProductionPlanning;

namespace RhyoliteOrbit.Services.ProductionPlanning.ProcessFlows.Dto;

public class CreateProcessFlowInput
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsDefault { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public List<ProcessStep> Steps { get; set; }
}

public class UpdateProcessFlowInput: EntityDto<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsDefault { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public List<ProcessStep> Steps { get; set; }
}