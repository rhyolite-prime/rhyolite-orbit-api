using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.Payroll.EmployeeRanks.Dto;

public class CreateEmployeeRankInput
{
    public string Name { get; set; }
}

public class UpdateEmployeeRankInput: EntityDto<Guid>
{
    public string Name { get; set; }
}