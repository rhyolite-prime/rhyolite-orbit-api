using System;

namespace RhyoliteOrbit.Models.Shared;

public class RemittanceAccount
{
    public Guid Id { get; set; }
    public string AccountType { get; set; }
    public Guid BankId { get; set; }
    public string BankCode { get; set; }
    public string AccountProvider { get; set; }
    public string AccountName { get; set; }
    public string AccountNumber { get; set; }
    public Guid CountryId { get; set; }
    public string CountryName { get; set; }
}