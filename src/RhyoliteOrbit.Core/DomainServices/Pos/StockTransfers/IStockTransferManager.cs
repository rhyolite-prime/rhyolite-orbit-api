using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.StockTransfers;

public interface IStockTransferManager: IDomainService
{
    // Creation & Dispatch
    Task<StockTransfer> CreateTransferAsync(StockTransfer transfer);
    Task ApproveTransferAsync(Guid stockTransferId, long approvedByUserId);
    Task DispatchTransferAsync(Guid stockTransferId, long dispatchedByUserId);
    Task CancelTransferAsync(Guid stockTransferId, string reason);

    // Receiving & Reconciliation
    Task ReceiveTransferAsync(Guid stockTransferId, long receivedByUserId, List<StockTransferDetail> actualReceivedDetails);
    Task RejectTransferAsync(Guid stockTransferId, string reason);

    // Validations & Queries
    Task<bool> ValidateStockAvailabilityAsync(Guid sourceBranchId, List<StockTransferDetail> details);
    Task<List<StockTransfer>> GetPendingOutboundTransfersAsync(Guid sourceBranchId);
    Task<List<StockTransfer>> GetInTransitInboundTransfersAsync(Guid destinationBranchId);
}