using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Wssd;

public class WssdService: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Name { get; set; }
    
    [MaxLength(25)]
    public string UssdCode { get; set; }
    
    [MaxLength(100)]
    public string DisplayTitle { get; set; }
    
    [MaxLength(250)]
    public string Description { get; set; }
     
    public DateTime ActivatedOn { get; set; }
    
    [MaxLength(350)]
    public string LogoUrl { get; set; }
    
    public bool IsWssdActive { get; set; }
    
    public bool IsUssdActive { get; set; }
    
    public bool IsWhiteLabelled { get; set; }
    
    [MaxLength(50)]
    public string WhiteLabelPrimaryThemeColor { get; set; }
    
    [MaxLength(50)]
    public string WhiteLabelSecondaryThemeColor { get; set; }
    
    [MaxLength(50)]
    public string CustomSubdomain { get; set; }
    
    [MaxLength(100)]
    public string ClientId { get; set; }
    
    [MaxLength(200)]
    public string ClientSecret { get; set; }
    
    [Column(TypeName = "jsonb")] public List<string> AllowedOrigins { get; set; }
    
    public bool UseAsSecondaryService { get; set; }
    
    public Guid MerchantId { get; set; }
    
    [MaxLength(100)]
    public string MerchantName { get; set; }
    
    [MaxLength(80)]
    public string Country { get; set; }
    
    [MaxLength(20)]
    public string CountryCode { get; set; }
    
    [MaxLength(50)]
    public string SubscriptionPlan { get; set; }
    
    [MaxLength(50)]
    public string BillingCycle { get; set; }
    
    [MaxLength(50)]
    public string Duration { get; set; }
    
    public decimal Fee { get; set; }
    
    public decimal UssdSessionRate { get; set; }
    
    public decimal WssdSessionRate { get; set; }
    
    public int SubscriptionDuration { get; set; } //in day units
    
    public DateTime LastRenewalDate { get; set; }
    
    [MaxLength(350)]
    public string WebhookUrl { get; set; }
    
    public decimal UssdSessionBalance { get; set; } 
    
    public decimal WssdSessionBalance { get; set; } 
    
    public int TenantId { get; set; }
}