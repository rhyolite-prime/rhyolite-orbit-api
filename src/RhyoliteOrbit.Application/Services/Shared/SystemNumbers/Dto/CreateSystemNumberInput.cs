using System.Collections.Generic;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.Services.Shared.SystemNumbers.Dto;

public class CreateSystemNumberInput
{
    public List<SystemNumber> SystemNumberList { get; set; }
}