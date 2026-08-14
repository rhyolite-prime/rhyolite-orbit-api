using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Pos;

public class CreditTerm: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Code { get; set; }
    
    [MaxLength(100)]
    public string Name { get; set; }
    
    [MaxLength(250)]
    public string Description { get; set; }
    
    // Term configuration
    public int TermDays { get; set; }
    
    public decimal DefaultInterestRate { get; set; }
    
    public decimal DefaultPenaltyRate { get; set; }
    
    public int GracePeriodDays { get; set; }

    public int DefaultNumberOfInstallments { get; set; }
    
    // Type and status
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; }
    
    public decimal MinAmount { get; set; }
    
    public decimal MaxAmount { get; set; }
    
    public int MinCreditScore { get; set; }
    
    public int TenantId { get; set; }

}