using System;

namespace RhyoliteOrbit.Models.Pos;

public class CouponAssignment
{
    public Guid Id { get; set; }
    
    public string CustomerPhoneNumber { get; set; }
    
    public string CustomerName { get; set; }
        
    public Guid CustomerId { get; set; }
    
    public int UsageCount { get; set; }
    
    public DateTime AssignedAt { get; set; }
}