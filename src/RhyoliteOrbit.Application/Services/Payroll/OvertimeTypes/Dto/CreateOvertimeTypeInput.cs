using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.Services.Payroll.OvertimeTypes.Dto;

public class CreateOvertimeTypeInput
{
    public string Name { get; set; }
    public bool Taxable { get; set; }
    public List<OvertimeRate> Rates { get; set; }
}

public class UpdateOvertimeTypeInput: EntityDto<Guid>
{
    public string Name { get; set; }
    public bool Taxable { get; set; }
    public List<OvertimeRate> Rates { get; set; }
}