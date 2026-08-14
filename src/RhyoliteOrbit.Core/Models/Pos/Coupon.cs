using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RhyoliteOrbit.Models.Pos;

public class Coupon: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public decimal CouponValue { get; set; }
    
    public decimal MinimumOrderLimit { get; set; }
    
    public decimal MaximumOrderLimit { get; set; }
    
    [MaxLength(10)]
    public string CouponIdentifier { get; set; }
    
    public DateTime ValidTill { get; set; }
    
    public int UsageQuota { get; set; }
    
    public int UsageCount { get; set; }
    
    public int Status { get; set; } // active, expired, 
    
    public bool UsePercentage { get; set; }
    
    public Guid BranchId { get; set; }
    
    [MaxLength(200)]
    public string BranchName { get; set; }
    
    [Column(TypeName = "jsonb")] public List<CouponAssignment> Assignments { get; set; }
    
    public int TenantId { get; set; }
}