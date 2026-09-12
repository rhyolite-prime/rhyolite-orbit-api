using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.StockReceipts;

public interface IStockReceiptManager: IDomainService
{
    // Receipt Lifecycle
    Task<StockReceipt> CreateReceiptFromTransferAsync(Guid stockTransferId, long receivedByUserId);
    Task ProcessAndCommitReceiptAsync(Guid stockReceiptId);
    Task CancelReceiptAsync(Guid stockReceiptId, string reason);

    // Quantity Verification & Discrepancies
    Task UpdateReceivedQuantitiesAsync(Guid stockReceiptId, List<StockReceiptDetail> updatedDetails);
    Task FlagDiscrepancyAsync(Guid stockReceiptId, string discrepancyNotes);

    // Queries
    Task<StockReceipt> GetByTransferNumberAsync(string transferNumber);
    Task<List<StockReceipt>> GetPendingReceiptsForBranchAsync(Guid receivingBranchId);
}