using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using RhyoliteOrbit.Models.ProductionPlanning;

namespace RhyoliteOrbit.Services.ProductionPlanning.QcChecklists.Dto;

public class CreateQcChecklistInput
{
    public string Name { get; set; }
    public string Description { get; set; }

    public Guid ProductId { get; set; }
    public string ProductName { get; set; }

    public List<QcChecklistItem> Items { get; set; }
}

public class UpdateQcChecklistInput: EntityDto<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }

    public Guid ProductId { get; set; }
    public string ProductName { get; set; }

    public List<QcChecklistItem> Items { get; set; }
}