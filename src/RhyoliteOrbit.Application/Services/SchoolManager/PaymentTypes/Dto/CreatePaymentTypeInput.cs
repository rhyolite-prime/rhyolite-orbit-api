using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.PaymentTypes.Dto;

public class CreatePaymentTypeInput
{
    public string Name { get; set; }
}
public class UpdatePaymentTypeInput: EntityDto<Guid>
{
    public string Name { get; set; }
}