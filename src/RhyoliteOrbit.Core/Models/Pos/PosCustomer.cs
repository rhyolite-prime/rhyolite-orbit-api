using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;

namespace RhyoliteOrbit.Models.Pos;

public class PosCustomer: FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Username { get; set; }    
    
    [MaxLength(100)]
    public string FirstName { get; set; }
    
    [MaxLength(100)]
    public string LastName { get; set; }
        
    [MaxLength(15)]
    public string PhoneNumber { get; set; }
    
    [MaxLength(250)]
    public string Pin { get; set; }
    
    [MaxLength(250)]
    public string Email { get; set; }
    
    public Guid RankId { get; set; }
    [MaxLength(100)]
    public string RankName { get; set; }
    
    public decimal TotalAmountSpent { get; set; }
    
    public int NumberOfVisits { get; set; }
    public int LoyaltyPoints { get; set; }

    [MaxLength(10)] public string DefaultCountry { get; set; }
        
    //credit profile settings
    public decimal CreditLimit { get; set; }
    
    public decimal WalletBalance { get; set; }
    
    public int DefaultCreditTermInDays { get; set; }
    
    public bool IsCreditAllowed { get; set; }
    
    public bool BlockedFromCredit { get; set; }
    
    public int TenantId { get; set; }
}