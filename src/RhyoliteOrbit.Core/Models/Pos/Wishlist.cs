using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Pos;

public class Wishlist: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid ItemId { get; set; }
    [MaxLength(350)]
    public string ItemImageUrl { get; set; }
    [MaxLength(200)]
    public string ItemName { get; set; }
    [MaxLength(30)]
    public string Sku { get; set; }
    public decimal UnitPrice { get; set; }
    public int MinOrderQuantity { get; set; }
    
    public Guid CustomerId { get; set; }
    public int TenantId { get; set; }
}