using System;
using System.ComponentModel.DataAnnotations;
using Abp.MultiTenancy;
using RhyoliteOrbit.Authorization.Users;

namespace RhyoliteOrbit.MultiTenancy;

public class Tenant : AbpTenant<User>
{
    [MaxLength(50)]
    public string BusinessId { get; set; }
    [MaxLength(150)]
    public string BusinessLogo { get; set; }
    
    public bool RequireTwoFactorAuth { get; set; }
     
    [MaxLength(20)]
    public string Country { get; set; }
    //subscription configuration
    //gl
    public bool HasSubscribedToGl { get; set; }
    
    public DateTime? GlSubscriptionStartDate { get; set; }
    public DateTime? GlSubscriptionEndDate { get; set; }
    public decimal GlSubscriptionFee{ get; set; }
    [MaxLength(50)]
    public string GlBillingCycle { get; set; }
    public DateTime? GlNextRenewalDate { get; set; }

    //payroll
    public bool HasSubscribedToPayroll { get; set; }
    public DateTime? PayrollSubscriptionStartDate { get; set; }
    public DateTime? PayrollSubscriptionEndDate { get; set; }
    public decimal PayrollSubscriptionFee{ get; set; }
    [MaxLength(50)]
    public string PayrollBillingCycle { get; set; }
    public DateTime? PayrollNextRenewalDate { get; set; }
    
    //school manager
    public bool HasSubscribedToSchoolManager { get; set; }
    public DateTime? SchoolManagerSubscriptionStartDate { get; set; }
    public DateTime? SchoolManagerSubscriptionEndDate { get; set; }
    public decimal SchoolManagerSubscriptionFee{ get; set; }
    [MaxLength(50)]
    public string SchoolManagerBillingCycle { get; set; }
    public DateTime? SchoolManagerNextRenewalDate { get; set; }
    
    //AP
    public bool HasSubscribedToAp { get; set; }
    public DateTime? ApSubscriptionStartDate { get; set; }
    public DateTime? ApSubscriptionEndDate { get; set; }
    public decimal ApSubscriptionFee{ get; set; }
    [MaxLength(50)]
    public string ApBillingCycle { get; set; }
    public DateTime? ApNextRenewalDate { get; set; }
    
    //AR
    public bool HasSubscribedToAr { get; set; }
    public DateTime? ArSubscriptionStartDate { get; set; }
    public DateTime? ArSubscriptionEndDate { get; set; }
    public decimal ArSubscriptionFee{ get; set; }
    [MaxLength(50)]
    public string ArBillingCycle { get; set; }
    public DateTime? ArNextRenewalDate { get; set; }
    
    //fixed assets
    public bool HasSubscribedToFixedAssets { get; set; }
    public DateTime? FixedAssetsSubscriptionStartDate { get; set; }
    public DateTime? FixedAssetsSubscriptionEndDate { get; set; }
    public decimal FixedAssetsSubscriptionFee{ get; set; }
    [MaxLength(50)]
    public string FixedAssetsBillingCycle { get; set; }
    public DateTime? FixedAssetsNextRenewalDate { get; set; }
    
    //human resource
    public bool HasSubscribedToHumanResource { get; set; }
    public DateTime? HumanResourceSubscriptionStartDate { get; set; }
    public DateTime? HumanResourceSubscriptionEndDate { get; set; }
    public decimal HumanResourceSubscriptionFee { get; set; }
    [MaxLength(50)]
    public string HumanResourceBillingCycle { get; set; }
    public DateTime? HumanResourceNextRenewalDate { get; set; }
     
    
    //Inventory
    public bool HasSubscribedToInventory { get; set; }
    public DateTime? InventorySubscriptionStartDate { get; set; }
    public DateTime? InventorySubscriptionEndDate { get; set; }
    public decimal InventorySubscriptionFee { get; set; }
    [MaxLength(50)]
    public string InventoryBillingCycle { get; set; }
    public DateTime? InventoryNextRenewalDate { get; set; }

    // manufacturing
    public bool HasSubscribedToManufacturing { get; set; }
    public DateTime? ManufacturingSubscriptionStartDate { get; set; }
    public DateTime? ManufacturingSubscriptionEndDate { get; set; }
    public decimal ManufacturingSubscriptionFee { get; set; }
    [MaxLength(50)]
    public string ManufacturingBillingCycle { get; set; }
    public DateTime? ManufacturingNextRenewalDate { get; set; }

    //point of sale
    public bool HasSubscribedToPointOfSale { get; set; }
    public DateTime? PointOfSaleSubscriptionStartDate { get; set; }
    public DateTime? PointOfSaleSubscriptionEndDate { get; set; }
    public decimal PointOfSaleSubscriptionFee { get; set; }
    [MaxLength(50)]
    public string PointOfSaleBillingCycle { get; set; }
    public DateTime? PointOfSaleNextRenewalDate { get; set; }

    //core banking
    public bool HasSubscribedToCoreBanking { get; set; }
    public DateTime? CoreBankingSubscriptionStartDate { get; set; }
    public DateTime? CoreBankingSubscriptionEndDate { get; set; }
    public decimal CoreBankingSubscriptionFee { get; set; }
    [MaxLength(50)]
    public string CoreBankingBillingCycle { get; set; }
    public DateTime? CoreBankingNextRenewalDate { get; set; }
    
    //Supply Chain
    public bool HasSubscribedToSupplyChain { get; set; }
    public DateTime? SupplyChainSubscriptionStartDate { get; set; }
    public DateTime? SupplyChainSubscriptionEndDate { get; set; }
    public decimal SupplyChainSubscriptionFee{ get; set; }
    [MaxLength(50)]
    public string SupplyChainBillingCycle { get; set; }
    public DateTime? SupplyChainNextRenewalDate { get; set; }
    
    //property & rentals
    public bool HasSubscribedToPropertyRentals { get; set; }
    public DateTime? PropertyRentalsSubscriptionStartDate { get; set; }
    public DateTime? PropertyRentalsSubscriptionEndDate { get; set; }
    public decimal PropertyRentalsSubscriptionFee{ get; set; }
    [MaxLength(50)]
    public string PropertyRentalsBillingCycle { get; set; }
    public DateTime? PropertyRentalsNextRenewalDate { get; set; }
    
    //document management
    public bool HasSubscribedToDocumentManagement { get; set; }
    public DateTime? DocumentManagementSubscriptionStartDate { get; set; }
    public DateTime? DocumentManagementSubscriptionEndDate { get; set; }
    public decimal DocumentManagementSubscriptionFee{ get; set; }
    [MaxLength(50)]
    public string DocumentManagementBillingCycle { get; set; }
    public DateTime? DocumentManagementNextRenewalDate { get; set; }
    
    public bool IsDirectDebitEnabled { get; set; }
   
    [MaxLength(20)]
    public string PhoneNumber { get; set; }
    [MaxLength(30)]
    public string PaymentMethod { get; set; } // mobile-money,bank-card
    [MaxLength(30)]
    public string MobileMoneyChannel { get; set; } //mtn-gh-direct-debit, vodafone-gh-direct-debit
    [MaxLength(20)]
    public string Network { get; set; }
    
    public Tenant()
    {
    }

    public Tenant(string tenancyName, string name)
        : base(tenancyName, name)
    {
    }
    
    
}
