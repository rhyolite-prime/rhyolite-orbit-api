using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Pos;

public class RefundAndReturn: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [Required]
    [MaxLength(30)]
    public string RequestNo { get; set; } // e.g., RET-2026-0001

    public Guid OrderId { get; set; }

    [Required]
    [MaxLength(30)]
    public string ReceiptNo { get; set; }

    public DateTime RequestDate { get; set; } = DateTime.UtcNow;

    // --- Branch & Customer ---
    public Guid BranchId { get; set; }

    public Guid? CustomerId { get; set; }

    [MaxLength(100)]
    public string CustomerName { get; set; }

    [MaxLength(20)]
    public string CustomerPhoneNo { get; set; }

    // --- Workflow & Resolution ---
    public RhyoliteOrbitConsts.RefundStatus Status { get; set; } = RhyoliteOrbitConsts.RefundStatus.Pending;

    public RhyoliteOrbitConsts.RequestedAction Action { get; set; } = RhyoliteOrbitConsts.RequestedAction.Refund;

    public RhyoliteOrbitConsts.RefundPaymentMethod PaymentMethod { get; set; } = RhyoliteOrbitConsts.RefundPaymentMethod.OriginalPaymentMethod;

    [MaxLength(250)]
    public string Reason { get; set; }

    public bool RestockItems { get; set; } = true; // Indicates whether returned items go back to live inventory

    // --- Financials ---
    public decimal RefundAmount { get; set; }

    // --- Auditing & Approvals ---
    public long? ApprovedByUserId { get; set; }

    public DateTime? ProcessedDate { get; set; }

    [MaxLength(250)]
    public string AdminNotes { get; set; }

    // --- Details ---
    [Column(TypeName = "jsonb")]
    public List<RefundItemDetail> Items { get; set; } = new();

    public int TenantId { get; set; }
}