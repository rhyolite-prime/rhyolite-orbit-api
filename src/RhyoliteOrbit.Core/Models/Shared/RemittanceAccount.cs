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
    public string CountryName { get; set; }
    public string CountryCode { get; set; }
    public bool IsDefault { get; set; }
}

public class ComplianceDocument
{
    public Guid Id { get; set; }
    public string DocumentName { get; set; }
    public string DocumentUrl { get; set; }
    public bool IsAccepted { get; set; }
     
}