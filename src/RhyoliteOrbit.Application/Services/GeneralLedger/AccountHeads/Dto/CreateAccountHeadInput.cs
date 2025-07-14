using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.GeneralLedger.AccountHeads.Dto;

public class CreateAccountHeadInput
{
    public string Name { get; set; }
    public int AccountHeadType { get; set; }
    public Guid ParentId { get; set; }
}

public class UpdateAccountHeadInput : EntityDto<Guid>
{
    public string Name { get; set; }
    public int AccountHeadType { get; set; }
    public Guid ParentId { get; set; }
}