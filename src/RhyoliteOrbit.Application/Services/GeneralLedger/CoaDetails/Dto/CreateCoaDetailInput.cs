using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.GeneralLedger.CoaDetails.Dto;

public class CreateCoaDetailInput
{
    public string AccountName { get; set; }
    public int AccountNo { get; set; }
    public RhyoliteOrbitConsts.AccountStatus Status { get; set; }
    public Guid AccountControlId { get; set; }
    public string AccountControlName { get; set; }
    public Guid CurrencyId { get; set; }
    public string CurrencyName { get; set; }
    public string CurrencyCode { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal CurrentForeignBalance { get; set; }
}

public class UpdateCoaDetailInput: EntityDto<Guid>
{
    public string AccountName { get; set; }
    public int AccountNo { get; set; }
    public RhyoliteOrbitConsts.AccountStatus Status { get; set; }
    public Guid AccountControlId { get; set; }
    public string AccountControlName { get; set; }
    public Guid CurrencyId { get; set; }
    public string CurrencyName { get; set; }
    public string CurrencyCode { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal CurrentForeignBalance { get; set; }
}