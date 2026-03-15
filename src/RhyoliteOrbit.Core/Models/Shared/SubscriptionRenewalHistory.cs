using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Shared;

public class SubscriptionRenewalHistory:Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
 
    public Guid SubscriptionPaymentId { get; set; }

    [MaxLength(50)]
    public string Combo { get; set; }

    [MaxLength(20)]
    public string Country { get; set; }

    [MaxLength(50)]
    public string Cycle { get; set; }

    [MaxLength(50)]
    public string Reference { get; set; }

    [MaxLength(20)]
    public string CustomerMsisdn { get; set; }
    
    public bool IsDirectDebitRenewal { get; set; }

    [MaxLength(50)]
    public string Channel { get; set; }
    
    [MaxLength(30)]
    public string PaymentMethod { get; set; }

    public decimal Amount { get; set; }

    public int Status { get; set; }

    // Renewal window
    public DateTime RenewalStartDate { get; set; }
    
    public DateTime RenewalEndDate { get; set; }

    // Optional operational fields
    [MaxLength(300)]
    public string Notes { get; set; }
    
    [Column(TypeName = "jsonb")] public List<int> SelectedModules { get; set; }

    public int TenantId { get; set; }
}