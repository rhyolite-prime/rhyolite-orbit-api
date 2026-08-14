using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Pos;

public class CreditAdjustment: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid CreditSaleId { get; set; }
    public decimal AdjustmentDate { get; set; }
    public decimal Amount { get; set; }
    [MaxLength(150)]
    public string Reason { get; set; }
    public int AdjustmentType { get; set; }
    
    public int TenantId { get; set; }
}