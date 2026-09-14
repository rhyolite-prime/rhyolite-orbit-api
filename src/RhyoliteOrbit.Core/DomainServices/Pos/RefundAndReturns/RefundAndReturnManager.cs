using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using RhyoliteOrbit.DomainServices.Pos.Customers;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.RefundAndReturns;

public class RefundAndReturnManager: DomainService, IRefundAndReturnManager
{
    private readonly IRepository<RefundAndReturn, Guid> _refundRepository;
    private readonly IRepository<PosOrder, Guid> _orderRepository;
    private readonly IRepository<PosInventoryItem, Guid> _branchInventoryRepository;
    private readonly ICustomerManager _customerManager;

    public RefundAndReturnManager(
        IRepository<RefundAndReturn, Guid> refundRepository,
        IRepository<PosOrder, Guid> orderRepository,
        IRepository<PosInventoryItem, Guid> branchInventoryRepository,
        ICustomerManager customerManager)
    {
        _refundRepository = refundRepository;
        _orderRepository = orderRepository;
        _branchInventoryRepository = branchInventoryRepository;
        _customerManager = customerManager;
    }

    #region Request Lifecycle

    public async Task<RefundAndReturn> CreateRequestAsync(RefundAndReturn request)
    {
        if (request.Items == null || !request.Items.Any())
        {
            throw new UserFriendlyException(400, "Return request must contain at least one item.");
        }

        // Validate eligibility against original order
        await ValidateReturnEligibilityAsync(request.OrderId, request.Items);

        // Calculate total refund value
        request.RefundAmount = request.Items.Sum(x => x.TotalPrice);
        request.RequestNo = $"RET-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
        request.RequestDate = DateTime.UtcNow;
        request.Status = RhyoliteOrbitConsts.RefundStatus.Pending;

        return await _refundRepository.InsertAsync(request);
    }

    public async Task ApproveRequestAsync(Guid refundRequestId, long approvedByUserId, string adminNotes = null)
    {
        var request = await GetRefundByIdAsync(refundRequestId);

        if (request.Status != RhyoliteOrbitConsts.RefundStatus.Pending)
        {
            throw new UserFriendlyException(400, "Only pending return requests can be approved.");
        }

        request.Status = RhyoliteOrbitConsts.RefundStatus.Approved;
        request.ApprovedByUserId = approvedByUserId;
        if (!string.IsNullOrWhiteSpace(adminNotes))
        {
            request.AdminNotes = adminNotes;
        }

        await _refundRepository.UpdateAsync(request);
    }

    public async Task RejectRequestAsync(Guid refundRequestId, long rejectedByUserId, string reason)
    {
        var request = await GetRefundByIdAsync(refundRequestId);

        if (request.Status == RhyoliteOrbitConsts.RefundStatus.Completed)
        {
            throw new UserFriendlyException(400, "Cannot reject an already completed refund.");
        }

        request.Status = RhyoliteOrbitConsts.RefundStatus.Rejected;
        request.ApprovedByUserId = rejectedByUserId;
        request.AdminNotes = string.IsNullOrWhiteSpace(request.AdminNotes)
            ? $"Rejected: {reason}"
            : $"{request.AdminNotes} | Rejected: {reason}";

        await _refundRepository.UpdateAsync(request);
    }

    public async Task ProcessRefundAsync(Guid refundRequestId, long processedByUserId)
    {
        var request = await GetRefundByIdAsync(refundRequestId);

        if (request.Status == RhyoliteOrbitConsts.RefundStatus.Completed)
        {
            throw new UserFriendlyException(400, "Refund has already been processed and completed.");
        }

        if (request.Status == RhyoliteOrbitConsts.RefundStatus.Rejected)
        {
            throw new UserFriendlyException(400, "Cannot process a rejected return request.");
        }

        var order = await _orderRepository.FirstOrDefaultAsync(request.OrderId);
        if (order == null)
        {
            throw new UserFriendlyException(404, "Associated POS Order was not found.");
        }

        // 1. Process Stock Restocking into Branch Inventory (if enabled)
        if (request.RestockItems)
        {
            foreach (var item in request.Items)
            {
                if (item.QuantityReturned <= 0) continue;

                var inventory = await _branchInventoryRepository.FirstOrDefaultAsync(
                    x => x.BranchId == request.BranchId && x.Id == item.ItemId
                );

                if (inventory != null)
                {
                    inventory.StockQuantity += item.QuantityReturned;
                    await _branchInventoryRepository.UpdateAsync(inventory);
                }
                else
                {
                    await _branchInventoryRepository.InsertAsync(new PosInventoryItem
                    {
                        BranchId = request.BranchId,
                       
                    });
                }
            }
        }

        // 2. Financial Disbursement / Payment Adjustments
        switch (request.PaymentMethod)
        {
            case RhyoliteOrbitConsts.RefundPaymentMethod.CustomerWallet:
            case RhyoliteOrbitConsts.RefundPaymentMethod.StoreCredit:
                if (!request.CustomerId.HasValue)
                {
                    throw new UserFriendlyException(400, "Customer account is required for wallet or store credit refunds.");
                }

                await _customerManager.AdjustWalletBalance(
                    request.CustomerId.Value,
                    request.RefundAmount,
                    $"Refund credit for return request {request.RequestNo}"
                );
                break;

            case RhyoliteOrbitConsts.RefundPaymentMethod.OriginalPaymentMethod:
                if (order.PaymentMethod.Equals("CustomerWallet", StringComparison.OrdinalIgnoreCase) && request.CustomerId.HasValue)
                {
                    await _customerManager.AdjustWalletBalance(
                        request.CustomerId.Value,
                        request.RefundAmount,
                        $"Original method refund for return request {request.RequestNo}"
                    );
                }
                // Handle external payment gateway API refunds here if applicable
                break;

            case RhyoliteOrbitConsts.RefundPaymentMethod.Cash:
                // Recorded as physical cash payout from drawer
                break;
        }

        // 3. Update Order Status & Refund Record
        var previousRefunds = await _refundRepository.GetAllListAsync(
            x => x.OrderId == request.OrderId && x.Status == RhyoliteOrbitConsts.RefundStatus.Completed && x.Id != request.Id
        );

        int totalItemsReturned = previousRefunds.SelectMany(x => x.Items).Sum(i => i.QuantityReturned) 
                                 + request.Items.Sum(i => i.QuantityReturned);
        int totalOriginalItems = order.OrderItems.Sum(i => i.Quantity);

        bool isFullRefund = totalItemsReturned >= totalOriginalItems;

        order.OrderStatus = isFullRefund ? RhyoliteOrbitConsts.OrderStatus.Refunded : RhyoliteOrbitConsts.OrderStatus.PartiallyRefunded;
        order.PaymentStatus = isFullRefund ? RhyoliteOrbitConsts.PaymentStatus.Refunded : RhyoliteOrbitConsts.PaymentStatus.PartiallyPaid;
        await _orderRepository.UpdateAsync(order);

        // Mark return request completed
        request.Status = RhyoliteOrbitConsts.RefundStatus.Completed;
        request.ProcessedDate = DateTime.UtcNow;
        request.ApprovedByUserId ??= processedByUserId;

        await _refundRepository.UpdateAsync(request);
    }

    #endregion

    #region Validation & Queries

    public async Task<bool> ValidateReturnEligibilityAsync(Guid orderId, List<RefundItemDetail> itemsToReturn)
    {
        var order = await _orderRepository.FirstOrDefaultAsync(orderId);
        if (order == null)
        {
            throw new UserFriendlyException(404, "Original POS order not found.");
        }

        if (order.OrderStatus == RhyoliteOrbitConsts.OrderStatus.Cancelled)
        {
            throw new UserFriendlyException(400, "Cannot process returns for a cancelled order.");
        }

        // Fetch completed historical returns for this order
        var existingRefunds = await _refundRepository.GetAllListAsync(
            x => x.OrderId == orderId && (x.Status == RhyoliteOrbitConsts.RefundStatus.Completed || x.Status == RhyoliteOrbitConsts.RefundStatus.Approved)
        );

        foreach (var item in itemsToReturn)
        {
            var originalItem = order.OrderItems.FirstOrDefault(x => x.ItemId == item.ItemId);
            if (originalItem == null)
            {
                throw new UserFriendlyException(400, $"Item '{item.ItemName}' was not part of original order {order.ReceiptNo}.");
            }

            int alreadyReturnedQty = existingRefunds
                .SelectMany(r => r.Items)
                .Where(i => i.ItemId == item.ItemId)
                .Sum(i => i.QuantityReturned);

            int maxRefundableQty = originalItem.Quantity - alreadyReturnedQty;

            if (item.QuantityReturned > maxRefundableQty)
            {
                throw new UserFriendlyException(400, $"Cannot return {item.QuantityReturned} units of '{item.ItemName}'. Maximum remaining refundable quantity is {maxRefundableQty}.");
            }
        }

        return true;
    }

    public async Task<List<RefundAndReturn>> GetPendingRequestsByBranchAsync(Guid branchId)
    {
        return await _refundRepository.GetAllListAsync(
            x => x.BranchId == branchId && x.Status == RhyoliteOrbitConsts.RefundStatus.Pending
        );
    }

    public async Task<List<RefundAndReturn>> GetRequestsByCustomerAsync(Guid customerId)
    {
        return await _refundRepository.GetAllListAsync(x => x.CustomerId == customerId);
    }

    #endregion

    #region Private Helpers

    private async Task<RefundAndReturn> GetRefundByIdAsync(Guid refundRequestId)
    {
        var request = await _refundRepository.FirstOrDefaultAsync(refundRequestId);
        if (request == null)
        {
            throw new UserFriendlyException(404, "Refund and return record not found.");
        }
        return request;
    }

    #endregion
}