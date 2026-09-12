using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Pos;

public class StockTransfer: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
     
    [MaxLength(30)]
    public string TransferNumber { get; set; } // Unique reference (e.g., ST-2026-0001)

    public RhyoliteOrbitConsts.TransferStatus Status { get; set; } = RhyoliteOrbitConsts.TransferStatus.Draft;

    public DateTime? ReceivedDate { get; set; }

    // --- Source Branch ---
    public Guid FromBranchId { get; set; }

    [MaxLength(100)]
    public string FromBranchName { get; set; }

    // --- Destination Branch ---
    public Guid ToBranchId { get; set; }

    [MaxLength(100)]
    public string ToBranchName { get; set; }

    // --- Audit & People Tracking ---
    public long? DispatchedByUserId { get; set; }

    public long? ReceivedByUserId { get; set; }

    [MaxLength(250)]
    public string Notes { get; set; }

    // --- Totals & Details ---
    public decimal TotalValue { get; set; }

    [Column(TypeName = "jsonb")]
    public List<StockTransferDetail> Details { get; set; }

    public int TenantId { get; set; }
}