using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Pos;

public class GiftCard: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(50)]
    public string Code { get; set; }
    public decimal Balance { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime? RedeemedAt { get; set; }
    public int TenantId { get; set; }
}