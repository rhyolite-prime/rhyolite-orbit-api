using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.StockTransfers;

public class StockTransferManager: DomainService, IStockTransferManager
{
    private readonly IRepository<StockTransfer, Guid> _stockTransferRepository;
    private readonly IRepository<PosInventoryItem, Guid> _branchInventoryRepository;

    public StockTransferManager(
        IRepository<StockTransfer, Guid> stockTransferRepository,
        IRepository<PosInventoryItem, Guid> branchInventoryRepository)
    {
        _stockTransferRepository = stockTransferRepository;
        _branchInventoryRepository = branchInventoryRepository;
    }

     

    public async Task<StockTransfer> CreateTransferAsync(StockTransfer transfer)
    {
        if (transfer.FromBranchId == transfer.ToBranchId)
        {
            throw new UserFriendlyException(400, "Source and destination branches cannot be the same.");
        }

        if (transfer.Details == null || !transfer.Details.Any())
        {
            throw new UserFriendlyException(400, "Transfer must contain at least one detail item.");
        }

        // Assign automatic Transfer Number if not provided
        if (string.IsNullOrWhiteSpace(transfer.TransferNumber))
        {
            transfer.TransferNumber = $"ST-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
        }

        transfer.Status = RhyoliteOrbitConsts.TransferStatus.Draft;
        transfer.CreationTime = DateTime.UtcNow;
        transfer.TotalValue = transfer.Details.Sum(x => x.TotalCost);

        return await _stockTransferRepository.InsertAsync(transfer);
    }

    public async Task ApproveTransferAsync(Guid stockTransferId, long approvedByUserId)
    {
        var transfer = await GetTransferByIdAsync(stockTransferId);

        if (transfer.Status != RhyoliteOrbitConsts.TransferStatus.Draft)
        {
            throw new UserFriendlyException(400, "Only draft transfers can be approved.");
        }

        // Verify stock availability prior to approving
        var isAvailable = await ValidateStockAvailabilityAsync(transfer.FromBranchId, transfer.Details);
        if (!isAvailable)
        {
            throw new UserFriendlyException(400, "Insufficient stock available in source branch for one or more items.");
        }

        transfer.Status = RhyoliteOrbitConsts.TransferStatus.Requested;
        await _stockTransferRepository.UpdateAsync(transfer);
    }

    public async Task DispatchTransferAsync(Guid stockTransferId, long dispatchedByUserId)
    {
        var transfer = await GetTransferByIdAsync(stockTransferId);

        if (transfer.Status != RhyoliteOrbitConsts.TransferStatus.Requested && transfer.Status != RhyoliteOrbitConsts.TransferStatus.Draft)
        {
            throw new UserFriendlyException(400, "Transfer is not in a dispatchable state.");
        }

        // 1. Verify source stock availability again right before deduction
        foreach (var item in transfer.Details)
        {
            var inventory = await _branchInventoryRepository.FirstOrDefaultAsync(
                x => x.BranchId == transfer.FromBranchId && x.Id == item.ItemId
            );

            if (inventory == null || inventory.StockQuantity < item.QuantityTransferred)
            {
                throw new UserFriendlyException(400, $"Insufficient stock for item '{item.ItemName}' at the source branch.");
            }
        }

        // 2. Deduct inventory from the source branch
        foreach (var item in transfer.Details)
        {
            var inventory = await _branchInventoryRepository.FirstOrDefaultAsync(
                x => x.BranchId == transfer.FromBranchId && x.Id == item.ItemId
            );

            inventory.StockQuantity -= item.QuantityTransferred;
            await _branchInventoryRepository.UpdateAsync(inventory);
        }

        // 3. Update transfer metadata and mark as InTransit
        transfer.Status = RhyoliteOrbitConsts.TransferStatus.InTransit;
        transfer.DispatchedByUserId = dispatchedByUserId;

        await _stockTransferRepository.UpdateAsync(transfer);
    }

    public async Task CancelTransferAsync(Guid stockTransferId, string reason)
    {
        var transfer = await GetTransferByIdAsync(stockTransferId);

        if (transfer.Status == RhyoliteOrbitConsts.TransferStatus.InTransit)
        {
            throw new UserFriendlyException(400, "Cannot cancel a transfer that is already in transit. Process a return transfer instead.");
        }

        if (transfer.Status == RhyoliteOrbitConsts.TransferStatus.Received)
        {
            throw new UserFriendlyException(400, "Cannot cancel a completed transfer.");
        }

        transfer.Status = RhyoliteOrbitConsts.TransferStatus.Cancelled;
        transfer.Notes = string.IsNullOrWhiteSpace(transfer.Notes) 
            ? $"Cancelled: {reason}" 
            : $"{transfer.Notes} | Cancelled: {reason}";

        await _stockTransferRepository.UpdateAsync(transfer);
    }
    
    public async Task ReceiveTransferAsync(Guid stockTransferId, long receivedByUserId, List<StockTransferDetail> actualReceivedDetails)
    {
        var transfer = await GetTransferByIdAsync(stockTransferId);

        if (transfer.Status != RhyoliteOrbitConsts.TransferStatus.InTransit)
        {
            throw new UserFriendlyException(400, "Only in-transit transfers can be marked as received.");
        }

        // 1. Increment destination branch inventory based on actual received amounts
        foreach (var receivedItem in actualReceivedDetails)
        {
            if (receivedItem.QuantityTransferred <= 0) continue;

            var inventory = await _branchInventoryRepository.FirstOrDefaultAsync(x => x.BranchId == transfer.ToBranchId && x.Id == receivedItem.ItemId);

            if (inventory != null)
            {
                inventory.StockQuantity += receivedItem.QuantityTransferred;
                await _branchInventoryRepository.UpdateAsync(inventory);
            }
            else
            {
                await _branchInventoryRepository.InsertAsync(new PosInventoryItem
                {
                    BranchId = transfer.ToBranchId,
                     
                });
            }
        }

        // 2. Update transfer record with actual received items and mark complete
        transfer.Details = actualReceivedDetails;
        transfer.Status = RhyoliteOrbitConsts.TransferStatus.Received;
        transfer.ReceivedDate = DateTime.UtcNow;
        transfer.ReceivedByUserId = receivedByUserId;

        await _stockTransferRepository.UpdateAsync(transfer);
    }

    public async Task RejectTransferAsync(Guid stockTransferId, string reason)
    {
        var transfer = await GetTransferByIdAsync(stockTransferId);

        if (transfer.Status != RhyoliteOrbitConsts.TransferStatus.InTransit)
        {
            throw new UserFriendlyException(400, "Only in-transit transfers can be rejected.");
        }

        // Return stock back to source branch inventory upon rejection
        foreach (var item in transfer.Details)
        {
            var sourceInventory = await _branchInventoryRepository.FirstOrDefaultAsync(
                x => x.BranchId == transfer.FromBranchId && x.Id == item.ItemId
            );

            if (sourceInventory != null)
            {
                sourceInventory.StockQuantity += item.QuantityTransferred;
                await _branchInventoryRepository.UpdateAsync(sourceInventory);
            }
        }

        transfer.Status = RhyoliteOrbitConsts.TransferStatus.Rejected;
        transfer.Notes = string.IsNullOrWhiteSpace(transfer.Notes) 
            ? $"Rejected: {reason}" 
            : $"{transfer.Notes} | Rejected: {reason}";

        await _stockTransferRepository.UpdateAsync(transfer);
    }
    public async Task<bool> ValidateStockAvailabilityAsync(Guid sourceBranchId, List<StockTransferDetail> details)
    {
        foreach (var item in details)
        {
            var inventory = await _branchInventoryRepository.FirstOrDefaultAsync(
                x => x.BranchId == sourceBranchId && x.Id == item.ItemId
            );

            if (inventory == null || inventory.StockQuantity < item.QuantityTransferred)
            {
                return false;
            }
        }

        return true;
    }

    public async Task<List<StockTransfer>> GetPendingOutboundTransfersAsync(Guid sourceBranchId)
    {
        return await _stockTransferRepository.GetAllListAsync(
            x => x.FromBranchId == sourceBranchId && (x.Status == RhyoliteOrbitConsts.TransferStatus.Draft || x.Status == RhyoliteOrbitConsts.TransferStatus.Requested)
        );
    }

    public async Task<List<StockTransfer>> GetInTransitInboundTransfersAsync(Guid destinationBranchId)
    {
        return await _stockTransferRepository.GetAllListAsync(
            x => x.ToBranchId == destinationBranchId && x.Status == RhyoliteOrbitConsts.TransferStatus.InTransit
        );
    }
    
    private async Task<StockTransfer> GetTransferByIdAsync(Guid stockTransferId)
    {
        var transfer = await _stockTransferRepository.FirstOrDefaultAsync(stockTransferId);
        if (transfer == null)
        {
            throw new UserFriendlyException(404, "Stock transfer record not found.");
        }
        return transfer;
    }

     
}