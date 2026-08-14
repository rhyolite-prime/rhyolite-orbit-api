using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Pos;

public class ProductReview: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid BranchId { get; set; }
    
    [MaxLength(100)]
    public string BranchName { get; set; }
    
    public Guid ItemId { get; set; }
    [MaxLength(100)]
    public string ItemName { get; set; }
    
    [MaxLength(100)]
    public string CustomerName { get; set; }
    
    [MaxLength(15)]
    public string CustomerPhoneNo { get; set; }
    
    [MaxLength(100)]
    public string CustomerEmail { get; set; }
    
    [MaxLength(100)]
    public string Title { get; set; }
    
    [MaxLength(350)]
    public string ReviewText { get; set; }
    
    [MaxLength(350)]
    public string ReplyText { get; set; }
    
    public int Rating { get; set; } //1-5
    
    public int TenantId { get; set; }
}