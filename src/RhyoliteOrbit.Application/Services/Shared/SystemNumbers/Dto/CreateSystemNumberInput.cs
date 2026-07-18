using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.Services.Shared.SystemNumbers.Dto;

public class CreateSystemNumberInput
{
    public List<SystemNumber> SystemNumberList { get; set; }
}

public class UpdateSystemNumberInput : EntityDto<Guid>
{
    public int ModuleId { get; set; }
    
    public string ItemName { get; set; }
    public string Key { get; set; }
    public string Prefix { get; set; }
    
    public string Suffix { get; set; }
    
    public string FormatPattern { get; set; }
    
    public int Padding { get; set; }
    public int LastNo { get; set; }
}