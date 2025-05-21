using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Shared;

public class Supplier: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(50)]
    public string SupplierIdentifier { get; set; }
    public Guid SupplierGroupId { get; set; }
    [MaxLength(200)]
    public string SupplierGroupName { get; set; }
    [MaxLength(100)]
    public string Status { get; set; }
    public decimal Balance { get; set; }
    public Guid VatAccountId { get; set; }
    [MaxLength(200)]
    public string VatAccountName { get; set; }
    public Guid CreditAccountId { get; set; }
    [MaxLength(200)]
    public string CreditAccountName { get; set; }
    public Guid DebitAccountId { get; set; }
    [MaxLength(200)]
    public string DebitAccountName { get; set; }
    [MaxLength(100)]
    public string TinNumber { get; set; }
    [MaxLength(200)]
    public string Name { get; set; }
    [MaxLength(200)]
    public string Email { get; set; }
    [MaxLength(50)]
    public string PhoneNo { get; set; }
    public Guid CurrencyId { get; set; }
    [MaxLength(100)]
    public string CurrencyName { get; set; }
    [MaxLength(50)]
    public string CurrencyCode { get; set; }
    public decimal CreditLimit { get; set; }
    public Guid IrsAccountId { get; set; }
    [MaxLength(200)]
    public string IrsAccountName { get; set; }
    [Column(TypeName = "jsonb")] public List<SupplierContact> SupplierContacts { get; set; }
    [Column(TypeName = "jsonb")] public List<SupplierAddress> SupplierAddresses { get; set; }
    [Column(TypeName = "jsonb")] public List<RemittanceAccount> RemittanceAccounts { get; set; }
    public int TenantId { get; set; }
}