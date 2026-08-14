using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.Models.Pos;

public class Merchant: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(200)]
    public string Name { get; set; }
    [MaxLength(20)]
    public string MerchantIdentifier { get; set; }
    [MaxLength(500)]
    public string BusinessDescription { get; set; }
    [MaxLength(100)]
    public string BusinessClassification { get; set; }
    [MaxLength(350)]
    public string LogoUrl { get; set; }
    [MaxLength(20)]
    public string ContactNumber { get; set; } 
    [MaxLength(150)]
    public string ContactEmail { get; set; }
    [MaxLength(150)]
    public string ContactPerson { get; set; }
    [MaxLength(50)]
    public string TaxIdentificationNumber { get; set; }
    [MaxLength(50)]
    public string BusinessRegistrationNumber { get; set; }
    [MaxLength(100)]
    public string IdentityType { get; set; }
    [MaxLength(100)]
    public string IdentityCardNo { get; set; }
    
    [Column(TypeName = "jsonb")] public List<ComplianceDocument> ComplianceDocuments { get; set; }
    
    [Column(TypeName = "jsonb")] public List<RemittanceAccount> RemittanceAccounts { get; set; }
    [MaxLength(150)]
    public string BusinessSector { get; set; }
    [MaxLength(150)]
    public string CountryName { get; set; }
    [MaxLength(150)]
    public string RegionOrState { get; set; }
    [MaxLength(150)]
    public string City { get; set; }
    
    [MaxLength(250)]
    public string Address { get; set; }
    public bool IsFeeChargeOnCustomer { get; set; }
    public Guid RhyoliteCbsClientId { get; set; }
    
    public Guid RevenueAccountId { get; set; }
    
    [MaxLength(50)]
    public string RevenueAccountNo { get; set; }
    
    public Guid PayoutAccountId { get; set; }
    
    public Guid PreferredPaymentChannelId { get; set; }
    
    public Guid PreferredPaymentChannelName { get; set; }
    
    [MaxLength(50)]
    public string PayoutAccountNo { get; set; }
    public int AverageRating { get; set; }
    
    [MaxLength(20)]
    public string Pin { get; set; }
    public bool IsActive { get; set; }
    
    [MaxLength(150)]
    public string WebsiteUrl { get; set; }
    
    public DateTime DateEstablished { get; set; }
    public bool HasSubAccountEnabled { get; set; }
    
    public bool IsDirectDebitEnabled { get; set; }
    
    public int TenantId { get; set; }
}