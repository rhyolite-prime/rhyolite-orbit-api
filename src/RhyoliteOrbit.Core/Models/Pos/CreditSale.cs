using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Pos;

public class CreditSale: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid MerchantId { get; set; }
    
    [MaxLength(150)]
    public string MerchantName { get; set; }
    public decimal PrincipalAmount { get; set; }
    public decimal OutstandingAmount { get; set; }
    public Guid CreditTermId { get; set; }
    public int CreditTermDays { get; set; }
    public DateTime CreditStartDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal InterestRate { get; set; }
    public decimal PenaltyRate { get; set; }
    public int NumberOfInstallments { get; set; }
    public int Status { get; set; } // credit sale status enum
    [MaxLength(150)]
    public string CommentsOrNotes { get; set; }
    [MaxLength(50)]
    public string ClientReference { get; set; }
    public int TenantId { get; set; }
}