using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.GeneralLedger.CoaControls.Dto;

public class CreateCoaControlInput
{
    public string Name { get; set; }
    public int MinAccount { get; set; }
    public int MaxAccount { get; set; }
    public Guid AccountHeadId { get; set; }
    
    public int AccountHeadType { get; set; }
    
    public Guid ParentControlId { get; set; }
}

public class UpdateCoaControlInput: EntityDto<Guid>
{
    public string Name { get; set; }
    public int MinAccount { get; set; }
    public int MaxAccount { get; set; }
    public Guid AccountHeadId { get; set; }
    
    public int AccountHeadType { get; set; }
    
    public Guid ParentControlId { get; set; }
}