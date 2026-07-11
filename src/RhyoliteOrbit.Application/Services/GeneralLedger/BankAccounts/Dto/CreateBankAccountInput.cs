using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.GeneralLedger.BankAccounts.Dto;

public class CreateBankAccountInput
{
    public Guid BankId { get; set; }
    public string BankName { get; set; }
    public Guid BankBranchId { get; set; }
    public string BankBranchName { get; set; }
    public Guid AccountId { get; set; }
    public string AccountName { get; set; }
    public string Address { get; set; }
    public string Location { get; set; }
    public string AccountNo { get; set; }
    public string AccountType { get; set; }
    public Guid CurrencyId { get; set; }
    public string CurrencyName { get; set; }
    public string CurrencyCode { get; set; }
    public string ContactPerson { get; set; }
    public string ContactNo { get; set; }
    public string ChequeBookStart { get; set; }
    public string ChequeBookEnd { get; set; }
    public DateTime DateOpened { get; set; }
}

public class UpdateBankAccountInput : EntityDto<Guid>
{
    public Guid BankId { get; set; }
    public string BankName { get; set; }
    public Guid BankBranchId { get; set; }
    public string BankBranchName { get; set; }
    public Guid AccountId { get; set; }
    public string AccountName { get; set; }
    public string Address { get; set; }
    public string Location { get; set; }
    public string AccountNo { get; set; }
    public string AccountType { get; set; }
    public Guid CurrencyId { get; set; }
    public string CurrencyName { get; set; }
    public string CurrencyCode { get; set; }
    public string ContactPerson { get; set; }
    public string ContactNo { get; set; }
    public string ChequeBookStart { get; set; }
    public string ChequeBookEnd { get; set; }
    public DateTime DateOpened { get; set; }
}