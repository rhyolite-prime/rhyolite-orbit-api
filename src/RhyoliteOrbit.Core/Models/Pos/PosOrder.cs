using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Pos;

public class PosOrder: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
        public Guid BranchId { get; set; }
        [MaxLength(100)]
        public string BranchName { get; set; }
        [MaxLength(50)]
        public string SoldBy { get; set; }
        
        public Guid CustomerId { get; set; }
        
        public Guid MerchantId { get; set; }
        [MaxLength(150)]
        public string MerchantName { get; set; }
        [MaxLength(20)]
        public string MerchantPhoneNumber { get; set; }
        [MaxLength(150)]
        public string MerchantEmail { get; set; }
        [MaxLength(100)]
        public string CustomerName { get; set; }
        [MaxLength(15)]
        public string CustomerPhoneNo { get; set; }
        [MaxLength(100)]
        public string CustomerEmail { get; set; }
        [MaxLength(20)]
        public string OrderStatus { get; set; }
        
        public int PaymentStatus { get; set; }
        public decimal PaymentAmount { get; set; }
        [MaxLength(50)]
        public string PaymentMethod { get; set; }
        [MaxLength(50)]
        public string PaymentSignature { get; set; }
        
        [MaxLength(20)]
        public string ReceiptNo { get; set; }
        public decimal OrderSubtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal OrderTax { get; set; }
        public decimal OrderDiscount { get; set; }
        public decimal OrderTotal { get; set; }
        
        [MaxLength(50)]
        public string CustomOrderNumber { get; set; }
        
        [MaxLength(200)]
        public string OrderSummaryInfo { get; set; }
        
        public bool HasDelivery { get; set; }
        
        [Column(TypeName = "jsonb")] public List<OrderItem> OrderItems { get; set; }
        
        [MaxLength(15)]
        public string Source { get; set; } // instore, online, sales-bot
         
        [MaxLength(250)]
        public string DeliveryAddress { get; set; }
        public Guid DeliveryChannelId { get; set; }
        [MaxLength(100)]
        public string DeliveryChannelName { get; set; }
        public int TenantId { get; set; }
}