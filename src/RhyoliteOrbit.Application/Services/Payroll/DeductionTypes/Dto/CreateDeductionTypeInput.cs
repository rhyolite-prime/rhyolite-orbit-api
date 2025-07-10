using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.Services.Payroll.DeductionTypes.Dto;

public class CreateDeductionTypeInput
{
    public string Name { get; set; }
    public Guid LiabilityAccountId { get; set; }
    public string LiabilityAccountName { get; set; }
}

public class UpdateDeductionTypeInput: EntityDto<Guid>
{
    public string Name { get; set; }
    public Guid LiabilityAccountId { get; set; }
    public string LiabilityAccountName { get; set; }
}