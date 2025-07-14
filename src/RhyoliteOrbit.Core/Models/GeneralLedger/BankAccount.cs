using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.GeneralLedger;

public class BankAccount: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid BankId { get; set; }
    [MaxLength(100)]
    public string BankName { get; set; }
    public Guid BankBranchId { get; set; }
    [MaxLength(100)]
    public string BankBranchName { get; set; }
    public Guid AccountId { get; set; }
    [MaxLength(150)]
    public string AccountName { get; set; }
    [MaxLength(150)]
    public string Address { get; set; }
    [MaxLength(150)]
    public string Location { get; set; }
    [MaxLength(100)]
    public string AccountNo { get; set; }
    [MaxLength(50)]
    public string AccountType { get; set; }
    public Guid CurrencyId { get; set; }
    [MaxLength(100)]
    public string CurrencyName { get; set; }
    [MaxLength(20)]
    public string CurrencyCode { get; set; }
    [MaxLength(130)]
    public string ContactPerson { get; set; }
    [MaxLength(20)]
    public string ContactNo { get; set; }
    [MaxLength(20)]
    public string ChequeBookStart { get; set; }
    [MaxLength(20)]
    public string ChequeBookEnd { get; set; }
    public DateTime DateOpened { get; set; }
    public int TenantId { get; set; }
    
}