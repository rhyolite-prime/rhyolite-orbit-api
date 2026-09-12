using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.Services.Pos.Merchants.Dto;

public class MerchantDto
{
    public string Name { get; set; }
    public string MerchantIdentifier { get; set; }
    public string BusinessDescription { get; set; }
    public string BusinessClassification { get; set; }
    public string LogoUrl { get; set; }
    public string ContactNumber { get; set; } 
    public string ContactEmail { get; set; }
    public string ContactPerson { get; set; }
    public string TaxIdentificationNumber { get; set; }
    public string BusinessRegistrationNumber { get; set; }
    public string IdentityType { get; set; }
    public string IdentityCardNo { get; set; }
    
    public List<ComplianceDocument> ComplianceDocuments { get; set; }
    
    public List<RemittanceAccount> RemittanceAccounts { get; set; }
    public string BusinessSector { get; set; }
    public string CountryName { get; set; }
    public string RegionOrState { get; set; }
    public string City { get; set; }
    
    public string Address { get; set; }
    public bool IsFeeChargeOnCustomer { get; set; }
    public Guid RhyoliteCbsClientId { get; set; }
    
    public Guid RevenueAccountId { get; set; }
    
    public string RevenueAccountNo { get; set; }
    
    public Guid PayoutAccountId { get; set; }
    
    public Guid PreferredPaymentChannelId { get; set; }
    
    public Guid PreferredPaymentChannelName { get; set; }
    
    public string PayoutAccountNo { get; set; }
    public int AverageRating { get; set; }
    
    public string Pin { get; set; }
    public bool IsActive { get; set; }
    
    public string WebsiteUrl { get; set; }
    
    public DateTime DateEstablished { get; set; }
    public bool HasSubAccountEnabled { get; set; }
    
    public bool IsDirectDebitEnabled { get; set; }
    
}

public class UpdateMerchantComplianceDocumentDto: EntityDto<Guid>
{
    public List<ComplianceDocument> ComplianceDocuments{ get; set; }
}

public class UpdateMerchantLogoDto : EntityDto<Guid>
{
    public string LogoUrl { get; set; }
}

public class UpdateMerchantRemittanceAccountsDto: EntityDto<Guid>
{
    public List<RemittanceAccount> RemittanceAccounts { get; set; }
}