using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Pos;

public class Delivery: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(30)]
    public string TrackingNumber { get; set; } // e.g., DEL-2026-0001

    public Guid OrderId { get; set; }

    [MaxLength(30)]
    public string ReceiptNo { get; set; }

    // --- Recipient & Destination ---
    [Required]
    [MaxLength(100)]
    public string RecipientName { get; set; }

    [Required]
    [MaxLength(20)]
    public string RecipientPhone { get; set; }

    [MaxLength(20)]
    public string SecondaryContactNo { get; set; }

    [Required]
    [MaxLength(250)]
    public string DestinationAddress { get; set; }

    [MaxLength(50)]
    public string RegionOrState { get; set; }

    [MaxLength(100)]
    public string Country { get; set; }

    // --- Channel & Zones ---
    public Guid DeliveryChannelId { get; set; }

    [MaxLength(60)]
    public string DeliveryChannelName { get; set; } // e.g., "In-House Fleet", "DHL", "FedEx"

    public Guid? DeliveryZoneId { get; set; }

    [MaxLength(60)]
    public string DeliveryZoneName { get; set; }

    public Guid? PickupPointId { get; set; }

    [MaxLength(100)]
    public string PickupPointName { get; set; }

    // --- Rider / Courier Assignment ---
    public Guid? RiderId { get; set; }

    [MaxLength(100)]
    public string RiderName { get; set; }

    [MaxLength(20)]
    public string RiderPhone { get; set; }

    // --- Dispatch & Timing ---
    public DateTime? EstimatedDeliveryTime { get; set; }

    public DateTime? DispatchedTime { get; set; }

    public DateTime? DeliveredTime { get; set; }

    [MaxLength(10)]
    public string DeliveryConfirmationCode { get; set; } // OTP / Verification Code

    // --- Financials & Status ---
    public decimal DeliveryFee { get; set; }

    public RhyoliteOrbitConsts.DeliveryStatus Status { get; set; } = RhyoliteOrbitConsts.DeliveryStatus.Pending;

    [MaxLength(250)]
    public string ExtraNotes { get; set; }

    public int TenantId { get; set; }
}