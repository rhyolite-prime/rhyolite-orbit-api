using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Wssd;

public class WssdServiceRequest: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string WssdServiceName { get; set; }
    
    [MaxLength(25)]
    public string UssdCode { get; set; }    
    
    public bool IsUssdCodePorted { get; set; }
    
    [MaxLength(100)]
    public string DisplayTitle { get; set; }
    
    [MaxLength(250)]
    public string PurposeDescription { get; set; }
    
    [MaxLength(250)]
    public string ServiceDescription { get; set; }
    
    [MaxLength(350)]
    public string LogoUrl { get; set; }
    
    public Guid MerchantId { get; set; }
    
    [MaxLength(100)]
    public string MerchantName { get; set; }
    
    [MaxLength(80)]
    public string Country { get; set; }
    
    [MaxLength(20)]
    public string CountryCode { get; set; }
    
    //subscription details...
    
    [MaxLength(50)]
    public string SubscriptionPlan { get; set; }
    
    [MaxLength(50)]
    public string BillingCycle { get; set; }
    
    [MaxLength(50)]
    public string Duration { get; set; }
    
    public decimal Fee { get; set; }
    
    public decimal SessionRate { get; set; }
    
    [MaxLength(50)]
    public string CustomSubdomain { get; set; }
    
    [MaxLength(50)]
    public string WhiteLabelPrimaryThemeColor { get; set; }
    public int TenantId { get; set; }
}