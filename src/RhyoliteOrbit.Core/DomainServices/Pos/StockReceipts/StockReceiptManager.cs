using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.StockReceipts;

public class StockReceiptManager: DomainService, IStockReceiptManager
{
    private readonly IRepository<StockReceipt, Guid> _stockReceiptRepository;
    private readonly IRepository<StockTransfer, Guid> _stockTransferRepository;
    private readonly IRepository<PosInventoryItem, Guid> _branchPosInventoryItem;

    public StockReceiptManager(
        IRepository<StockReceipt, Guid> stockReceiptRepository,
        IRepository<StockTransfer, Guid> stockTransferRepository,
        IRepository<PosInventoryItem, Guid> branchPosInventoryItem)
    {
        _stockReceiptRepository = stockReceiptRepository;
        _stockTransferRepository = stockTransferRepository;
        _branchPosInventoryItem = branchPosInventoryItem;
    }

     

    public async Task<StockReceipt> CreateReceiptFromTransferAsync(Guid stockTransferId, long receivedByUserId)
    {
        var transfer = await _stockTransferRepository.FirstOrDefaultAsync(stockTransferId);
        if (transfer == null)
        {
            throw new UserFriendlyException(404, "Originating stock transfer not found.");
        }

        if (transfer.Status != RhyoliteOrbitConsts.TransferStatus.InTransit)
        {
            throw new UserFriendlyException(400, "Cannot create a receipt for a transfer that is not in-transit.");
        }

        // Check if receipt already exists
        var existingReceipt = await _stockReceiptRepository.FirstOrDefaultAsync(x => x.StockTransferId == stockTransferId);
        if (existingReceipt != null)
        {
            throw new UserFriendlyException(400, "A receipt record has already been created for this transfer.");
        }

        // Map transfer details to receipt details
        var receiptDetails = transfer.Details.Select(detail => new StockReceiptDetail
        {
            ItemId = detail.ItemId,
            Sku = detail.Sku,
            ItemName = detail.ItemName,
            UnitOfMeasure = detail.UnitOfMeasure,
            QuantityTransferred = detail.QuantityTransferred,
            QuantityReceived = detail.QuantityTransferred, // Default to full receipt, adjustable before committing
            UnitCost = detail.UnitCost,
            BatchNumber = detail.BatchNumber
        }).ToList();

        var receipt = new StockReceipt
        {
            ReceiptNumber = $"SR-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}",
            StockTransferId = transfer.Id,
            TransferNumber = transfer.TransferNumber,
            DateReceived = DateTime.UtcNow,
            ReceivingBranchId = transfer.ToBranchId,
            ReceivingBranchName = transfer.ToBranchName,
            DispatchingBranchId = transfer.FromBranchId,
            DispatchingBranchName = transfer.FromBranchName,
            ReceivedByUserId = receivedByUserId,
            Status = RhyoliteOrbitConsts.ReceiptStatus.Draft,
            TotalValue = receiptDetails.Sum(x => x.QuantityReceived * x.UnitCost),
            ReceiptDetails = receiptDetails
        };

        return await _stockReceiptRepository.InsertAsync(receipt);
    }

    public async Task ProcessAndCommitReceiptAsync(Guid stockReceiptId)
    {
        var receipt = await GetReceiptByIdAsync(stockReceiptId);

        if (receipt.Status == RhyoliteOrbitConsts.ReceiptStatus.Completed)
        {
            throw new UserFriendlyException(400, "Receipt has already been processed and committed.");
        }

        if (receipt.Status == RhyoliteOrbitConsts.ReceiptStatus.Cancelled)
        {
            throw new UserFriendlyException(400, "Cannot commit a cancelled receipt.");
        }

        // 1. Increment local inventory levels for the receiving branch
        foreach (var item in receipt.ReceiptDetails)
        {
            if (item.QuantityReceived <= 0) continue;

            var inventory = await _branchPosInventoryItem.FirstOrDefaultAsync(
                x => x.BranchId == receipt.ReceivingBranchId && x.Id == item.ItemId
            );

            if (inventory != null)
            {
                inventory.StockQuantity += item.QuantityReceived;
                await _branchPosInventoryItem.UpdateAsync(inventory);
            }
            else
            {
                // Initialize stock entry if item doesn't exist in branch inventory
                await _branchPosInventoryItem.InsertAsync(new PosInventoryItem
                {
                    BranchId = receipt.ReceivingBranchId,
                    StockQuantity = item.QuantityReceived
                });
            }
        }

        // 2. Check for variances between expected and received
        bool hasDiscrepancy = receipt.ReceiptDetails.Any(x => x.QuantityReceived != x.QuantityTransferred);
        receipt.Status = hasDiscrepancy ? RhyoliteOrbitConsts.ReceiptStatus.DiscrepancyFlagged : RhyoliteOrbitConsts.ReceiptStatus.Completed;
        receipt.AutoUpdateInventory = true;

        await _stockReceiptRepository.UpdateAsync(receipt);

        // 3. Complete linked StockTransfer record
        if (receipt.StockTransferId.HasValue)
        {
            var transfer = await _stockTransferRepository.FirstOrDefaultAsync(receipt.StockTransferId.Value);
            if (transfer != null)
            {
                transfer.Status = RhyoliteOrbitConsts.TransferStatus.Received;
                transfer.ReceivedDate = DateTime.UtcNow;
                transfer.ReceivedByUserId = receipt.ReceivedByUserId;
                await _stockTransferRepository.UpdateAsync(transfer);
            }
        }
    }

    public async Task CancelReceiptAsync(Guid stockReceiptId, string reason)
    {
        var receipt = await GetReceiptByIdAsync(stockReceiptId);

        if (receipt.Status == RhyoliteOrbitConsts.ReceiptStatus.Completed)
        {
            throw new UserFriendlyException(400, "Cannot cancel a receipt that has already updated inventory.");
        }

        receipt.Status = RhyoliteOrbitConsts.ReceiptStatus.Cancelled;
        receipt.Remarks = string.IsNullOrWhiteSpace(receipt.Remarks) 
            ? $"Cancelled: {reason}" 
            : $"{receipt.Remarks} | Cancelled: {reason}";

        await _stockReceiptRepository.UpdateAsync(receipt);
    }

    public async Task UpdateReceivedQuantitiesAsync(Guid stockReceiptId, List<StockReceiptDetail> updatedDetails)
    {
        var receipt = await GetReceiptByIdAsync(stockReceiptId);

        if (receipt.Status == RhyoliteOrbitConsts.ReceiptStatus.Completed)
        {
            throw new UserFriendlyException(400, "Cannot modify details of a committed receipt.");
        }

        receipt.ReceiptDetails = updatedDetails;
        receipt.TotalValue = updatedDetails.Sum(x => x.QuantityReceived * x.UnitCost);

        await _stockReceiptRepository.UpdateAsync(receipt);
    }

    public async Task FlagDiscrepancyAsync(Guid stockReceiptId, string discrepancyNotes)
    {
        var receipt = await GetReceiptByIdAsync(stockReceiptId);

        receipt.Status = RhyoliteOrbitConsts.ReceiptStatus.DiscrepancyFlagged;
        receipt.Remarks = discrepancyNotes;

        await _stockReceiptRepository.UpdateAsync(receipt);
    }
    

    public async Task<StockReceipt> GetByTransferNumberAsync(string transferNumber)
    {
        return await _stockReceiptRepository.FirstOrDefaultAsync(x => x.TransferNumber == transferNumber);
    }

    public async Task<List<StockReceipt>> GetPendingReceiptsForBranchAsync(Guid receivingBranchId)
    {
        return await _stockReceiptRepository.GetAllListAsync(
            x => x.ReceivingBranchId == receivingBranchId && x.Status == RhyoliteOrbitConsts.ReceiptStatus.Draft
        );
    }
    

    private async Task<StockReceipt> GetReceiptByIdAsync(Guid receiptId)
    {
        var receipt = await _stockReceiptRepository.FirstOrDefaultAsync(receiptId);
        if (receipt == null)
        {
            throw new UserFriendlyException(404, "Stock receipt not found.");
        }
        return receipt;
    }
}