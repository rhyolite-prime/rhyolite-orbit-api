using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.Shared.UnitOfMeasurements.Dto;

public class CreateUnitOfMeasureInput
{
    public string Name { get; set; }
    public string Description { get; set; }
}

public class UpdateUnitOfMeasureInput: EntityDto<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
}