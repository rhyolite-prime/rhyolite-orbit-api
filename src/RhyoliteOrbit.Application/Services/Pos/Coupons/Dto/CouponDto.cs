using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.Services.Pos.Coupons.Dto;

public class CouponDto
{
    public decimal CouponValue { get; set; }
    
    public decimal MinimumOrderLimit { get; set; }
    
    public decimal MaximumOrderLimit { get; set; }
    
    public string CouponIdentifier { get; set; }
    
    public DateTime ValidTill { get; set; }
    
    public int UsageQuota { get; set; }
    
    public int UsageCount { get; set; }
    
    public int Status { get; set; } // active, expired, 
    
    public bool UsePercentage { get; set; }
    
    public Guid BranchId { get; set; }
    
    public string BranchName { get; set; }
    
    public List<CouponAssignment> Assignments { get; set; }
}