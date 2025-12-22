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
    
    //Inventory
    
    
    //Supply Chain
    
    
    //property & rentals
    public bool HasSubscribedToPropertyRentals { get; set; }
    public DateTime? PropertyRentalsSubscriptionStartDate { get; set; }
    public DateTime? PropertyRentalsSubscriptionEndDate { get; set; }
    public decimal PropertyRentalsSubscriptionFee{ get; set; }
    [MaxLength(50)]
    public string PropertyRentalsBillingCycle { get; set; }
    public DateTime? PropertyRentalsNextRenewalDate { get; set; }
    
    
    
    public Tenant()
    {
    }

    public Tenant(string tenancyName, string name)
        : base(tenancyName, name)
    {
    }
    
    
}
