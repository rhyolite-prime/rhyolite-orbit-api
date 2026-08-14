using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Pos;

public class Delivery: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid OrderId { get; set; }
    [MaxLength(200)]
    public string ShippingMethod { get; set; }
    public DateTime PickupTime { get; set; }
    public DateTime DeliveryTime { get; set; }
    [MaxLength(10)]
    public string DeliveryConfirmationCode { get; set; }
    public bool IsDelivered { get; set; }
    [MaxLength(20)]
    public string RiderPhone { get; set; }
    [MaxLength(200)]
    public string RiderName { get; set; }
    public decimal Amount { get; set; }
    public int Status { get; set; } // delivery status enum
    [MaxLength(250)]
    public string DestinationAddress { get; set; }
    
    [Column(TypeName = "jsonb")] public List<OrderItem> OrderItems { get; set; }
    
    public Guid DeliveryChannelId { get; set; }
    
    [MaxLength(60)]
    public string DeliveryChannelName { get; set; }
    
    public Guid DeliveryZoneId { get; set; }
    
    [MaxLength(60)]
    public string DeliveryZoneName { get; set; }
    
    public decimal DeliveryFee { get; set; }
    
    [MaxLength(20)]
    public string SecondaryContactNo { get; set; }
    public Guid PickupPointId { get; set; }
    [MaxLength(50)]
    public string PickupPointName { get; set; }
    [MaxLength(250)]
    public string ExtraNotes { get; set; }
    [MaxLength(100)]
    public string Country { get; set; }
    [MaxLength(50)]
    public string RegionOrState { get; set; }
    public int TenantId { get; set; }
}