using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.Payroll.LoanTypes.Dto;

public class CreateLoanTypeInput
{
    public string Name { get; set; }
    public bool ChargeInterest { get; set; }
}

public class UpdateLoanTypeInput: EntityDto<Guid>
{
    public string Name { get; set; }
    public bool ChargeInterest { get; set; }
}