using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Pos;

public class PosOrder: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(30)]
    public string ReceiptNo { get; set; }

    [MaxLength(50)]
    public string CustomOrderNumber { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [MaxLength(20)]
    public string Source { get; set; } = "InStore"; // InStore, MobileApp, ECommerce, SalesBot

    // --- Branch & Staff ---
    public Guid BranchId { get; set; }

    [MaxLength(100)]
    public string BranchName { get; set; }

    [MaxLength(100)]
    public string SoldBy { get; set; } // Cashier or Sales Associate name/username

    // --- Customer Information ---
    public Guid? CustomerId { get; set; } // Nullable for guest cash walk-ins

    [MaxLength(100)]
    public string CustomerName { get; set; }

    [MaxLength(20)]
    public string CustomerPhoneNo { get; set; }

    [MaxLength(100)]
    public string CustomerEmail { get; set; }

    // --- Financial Breakdowns ---
    public decimal OrderSubtotal { get; set; }

    public decimal OrderTax { get; set; }

    public decimal OrderDiscount { get; set; }

    public decimal ShippingCost { get; set; }

    public decimal OrderTotal { get; set; }

    public decimal PaymentAmount { get; set; } // Amount tendered by customer

    public decimal ChangeGiven => PaymentAmount > OrderTotal ? PaymentAmount - OrderTotal : 0;

    public decimal BalanceDue => OrderTotal > PaymentAmount ? OrderTotal - PaymentAmount : 0;

    // --- Payment Details ---
    public RhyoliteOrbitConsts.PaymentStatus PaymentStatus { get; set; } = RhyoliteOrbitConsts.PaymentStatus.Pending;

    [MaxLength(50)]
    public string PaymentMethod { get; set; } // Cash, Card, MobileMoney, CustomerWallet, Credit

    [MaxLength(100)]
    public string PaymentReference { get; set; } // External Gateway transaction ID or approval code

    // --- Lifecycle & Delivery ---
    public RhyoliteOrbitConsts.OrderStatus OrderStatus { get; set; } = RhyoliteOrbitConsts.OrderStatus.Completed;

    public bool HasDelivery { get; set; }

    [MaxLength(250)]
    public string DeliveryAddress { get; set; }

    public Guid? DeliveryChannelId { get; set; }

    [MaxLength(100)]
    public string DeliveryChannelName { get; set; }

    // --- Items ---
    [Column(TypeName = "jsonb")] 
    public List<OrderItem> OrderItems { get; set; } = new();

    public int TenantId { get; set; }
}