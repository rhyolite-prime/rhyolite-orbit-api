using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.Services.Shared.Banks.Dto;

public class CreateBankInput
{
    public string Name { get; set; }
    public string ContactPerson { get; set; }
    public string ContactNo { get; set; }
    public string Email { get; set; }
    public string BankCode { get; set; }
    public List<BankBranch> BankBranches { get; set; }
}

public class UpdateBankInput : EntityDto<Guid>
{
    public string Name { get; set; }
    public string ContactPerson { get; set; }
    public string ContactNo { get; set; }
    public string Email { get; set; }
    public string BankCode { get; set; }
    public List<BankBranch> BankBranches { get; set; }
}

 