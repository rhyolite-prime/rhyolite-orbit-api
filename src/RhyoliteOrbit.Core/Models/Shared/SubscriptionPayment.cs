using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Shared;

public class SubscriptionPayment: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    
    [MaxLength(50)]
    public string Cycle { get; set; }
    
    [MaxLength(50)]
    public string Reference { get; set; }
    [MaxLength(20)]
    public string PhoneNumber { get; set; } // formatted to country code code
    [MaxLength(30)]
    public string PaymentMethod { get; set; } // mobile-money,bank-card
    
    [MaxLength(50)]
    public string Channel { get; set; } // for direct debit; mtn-gh-direct-debit, vodafone-gh-direct-debit
    public bool IsDirectDebitPayment { get; set; }
    
    [MaxLength(300)]
    public string Description { get; set; }
    
    public decimal Amount { get; set; }
    public int Status { get; set; }
     
    public int TenantId { get; set; }
}