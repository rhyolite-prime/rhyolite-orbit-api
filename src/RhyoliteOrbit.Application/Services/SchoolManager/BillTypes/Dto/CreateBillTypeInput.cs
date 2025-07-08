using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.BillTypes.Dto;

public class CreateBillTypeInput
{
    public string Name { get; set; }
}
public class UpdateBillTypeInput: EntityDto<Guid>
{
    public string Name { get; set; }
}