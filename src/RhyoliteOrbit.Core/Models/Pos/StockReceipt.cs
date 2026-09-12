using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Pos;

public class StockReceipt: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(30)]
    public string ReceiptNumber { get; set; } // e.g., SR-2026-0001

    public Guid? StockTransferId { get; set; } // Reference to source StockTransfer (if linked)

    [MaxLength(30)]
    public string TransferNumber { get; set; } // Cross-reference number from shipping branch

    public DateTime DateReceived { get; set; }

    // --- Branch Information ---
    public Guid ReceivingBranchId { get; set; }

    [MaxLength(100)]
    public string ReceivingBranchName { get; set; }

    public Guid DispatchingBranchId { get; set; }

    [MaxLength(100)]
    public string DispatchingBranchName { get; set; }

    // --- Status & Inventory Rules ---
    public bool AutoUpdateInventory { get; set; } = true; // Indicates whether stock was committed to live inventory

    public RhyoliteOrbitConsts.ReceiptStatus Status { get; set; } = RhyoliteOrbitConsts.ReceiptStatus.Completed;

    // --- Audit & Notes ---
    public long ReceivedByUserId { get; set; }

    [MaxLength(250)]
    public string Remarks { get; set; }

    // --- Totals & Details ---
    public decimal TotalValue { get; set; }

    [Column(TypeName = "jsonb")] 
    public List<StockReceiptDetail> ReceiptDetails { get; set; }

    public int TenantId { get; set; }
}