using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using RhyoliteOrbit.DomainServices.Pos.Customers;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.Orders;

public class OrderManager: DomainService, IOrderManager
{
    private readonly IRepository<PosOrder, Guid> _orderRepository;
    private readonly IRepository<Coupon, Guid> _couponRepository;
    private readonly IRepository<PosInventoryItem, Guid> _branchInventoryRepository;
    private readonly ICustomerManager _customerManager;

    public OrderManager(
        IRepository<PosOrder, Guid> orderRepository,
        IRepository<PosInventoryItem, Guid> branchInventoryRepository,
        ICustomerManager customerManager, IRepository<Coupon, Guid> couponRepository)
    {
        _orderRepository = orderRepository;
        _branchInventoryRepository = branchInventoryRepository;
        _customerManager = customerManager;
        _couponRepository = couponRepository;
    }

    #region Order Creation & Processing

    public async Task<PosOrder> CreateOrderAsync(PosOrder order)
    {
        if (order.OrderItems == null || !order.OrderItems.Any())
        {
            throw new UserFriendlyException(400, "Order must contain at least one item.");
        }

        // 1. Calculate Financial Totals
        order.OrderSubtotal = order.OrderItems.Sum(x => x.Quantity * x.UnitPrice);
        order.OrderTotal = (order.OrderSubtotal + order.OrderTax + order.ShippingCost) - order.OrderDiscount;

        if (order.OrderTotal < 0)
        {
            order.OrderTotal = 0;
        }

        // 2. Generate Receipt Number if missing
        if (string.IsNullOrWhiteSpace(order.ReceiptNo))
        {
            order.ReceiptNo = $"REC-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(10000, 99999)}";
        }

        order.OrderDate = DateTime.UtcNow;
        order.OrderStatus = RhyoliteOrbitConsts.OrderStatus.Pending;
        order.PaymentStatus = RhyoliteOrbitConsts.PaymentStatus.Pending;

        return await _orderRepository.InsertAsync(order);
    }

    public async Task ProcessPaymentAsync(Guid orderId, string paymentMethod, decimal amountTendered, string paymentReference)
    {
        var order = await GetOrderByIdAsync(orderId);

        if (order.PaymentStatus == RhyoliteOrbitConsts.PaymentStatus.Completed)
        {
            throw new UserFriendlyException(400, "Order has already been paid.");
        }

        if (order.OrderStatus == RhyoliteOrbitConsts.OrderStatus.Cancelled)
        {
            throw new UserFriendlyException(400, "Cannot process payment for a cancelled order.");
        }

        // 1. Check Store Credit / Wallet rules if customer is paying via wallet/credit
        if (paymentMethod.Equals("CustomerWallet", StringComparison.OrdinalIgnoreCase))
        {
            if (!order.CustomerId.HasValue)
            {
                throw new UserFriendlyException(400, "Customer account is required to pay via store wallet.");
            }

            await _customerManager.AdjustWalletBalanceAsync(
                order.CustomerId.Value, 
                -order.OrderTotal, 
                $"Payment for order {order.ReceiptNo}"
            );
        }
        else if (paymentMethod.Equals("Credit", StringComparison.OrdinalIgnoreCase))
        {
            if (!order.CustomerId.HasValue)
            {
                throw new UserFriendlyException(400, "Customer account is required for credit purchases.");
            }

            bool isEligible = await _customerManager.CanPurchaseOnCreditAsync(order.CustomerId.Value, order.OrderTotal);
            if (!isEligible)
            {
                throw new UserFriendlyException(400, "Customer is not eligible for this credit purchase amount.");
            }
        }

        // 2. Verify Stock Availability and Deduct Stock
        foreach (var item in order.OrderItems)
        {
            var inventory = await _branchInventoryRepository.FirstOrDefaultAsync(
                x => x.BranchId == order.BranchId && x.Id == item.ItemId
            );

            if (inventory == null || inventory.StockQuantity < item.Quantity)
            {
                throw new UserFriendlyException(400, $"Insufficient stock for item '{item.ItemName}' at the current branch.");
            }

            // Deduct inventory
            inventory.StockQuantity -= item.Quantity;
            await _branchInventoryRepository.UpdateAsync(inventory);
        }

        // 3. Update Order Payment & Completion Status
        order.PaymentMethod = paymentMethod;
        order.PaymentAmount = amountTendered;
        order.PaymentReference = paymentReference;
        order.PaymentStatus = amountTendered >= order.OrderTotal ? RhyoliteOrbitConsts.PaymentStatus.Completed : RhyoliteOrbitConsts.PaymentStatus.PartiallyPaid;
        order.OrderStatus = RhyoliteOrbitConsts.OrderStatus.Completed;

        await _orderRepository.UpdateAsync(order);

        // 4. Accumulate Customer Loyalty Metrics
        if (order.CustomerId.HasValue)
        {
            await _customerManager.RecordPurchaseAsync(order.CustomerId.Value, order.OrderTotal);
            
            // Example rule: 1 point for every $10 spent
            int pointsEarned = (int)(order.OrderTotal / 10);
            if (pointsEarned > 0)
            {
                await _customerManager.AddLoyaltyPointsAsync(order.CustomerId.Value, pointsEarned);
            }
        }
    }

    #endregion

    #region Order Cancellation & Refunds

    public async Task CancelOrderAsync(Guid orderId, string reason)
    {
        var order = await GetOrderByIdAsync(orderId);

        if (order.OrderStatus == RhyoliteOrbitConsts.OrderStatus.Completed && order.PaymentStatus == RhyoliteOrbitConsts.PaymentStatus.Completed)
        {
            throw new UserFriendlyException(400, "Completed paid orders cannot be directly cancelled. Use Refund instead.");
        }

        order.OrderStatus = RhyoliteOrbitConsts.OrderStatus.Cancelled;
        await _orderRepository.UpdateAsync(order);
    }

    public async Task RefundOrderAsync(Guid orderId, List<RefundRequestDto> refundItems, string reason, long processedByUserId)
    {
        var order = await GetOrderByIdAsync(orderId);

        if (order.OrderStatus != RhyoliteOrbitConsts.OrderStatus.Completed)
        {
            throw new UserFriendlyException(400, "Only completed orders can be refunded.");
        }

        decimal totalRefundAmount = 0;

        foreach (var refundItem in refundItems)
        {
            var lineItem = order.OrderItems.FirstOrDefault(x => x.ItemId == refundItem.ItemId);
            if (lineItem == null)
            {
                throw new UserFriendlyException(400, $"Item ID {refundItem.ItemId} is not part of this order.");
            }

            if (refundItem.QuantityToRefund > lineItem.Quantity)
            {
                throw new UserFriendlyException(400, $"Cannot refund more items than were originally purchased for '{lineItem.ItemName}'.");
            }

            // 1. Restore Inventory back to Branch
            var inventory = await _branchInventoryRepository.FirstOrDefaultAsync(
                x => x.BranchId == order.BranchId && x.Id == refundItem.ItemId
            );

            if (inventory != null)
            {
                inventory.StockQuantity += refundItem.QuantityToRefund;
                await _branchInventoryRepository.UpdateAsync(inventory);
            }

            totalRefundAmount += refundItem.QuantityToRefund * lineItem.UnitPrice;
        }

        // 2. Adjust Customer Wallet if originally paid via Wallet
        if (order.PaymentMethod.Equals("CustomerWallet", StringComparison.OrdinalIgnoreCase) && order.CustomerId.HasValue)
        {
            await _customerManager.AdjustWalletBalanceAsync(
                order.CustomerId.Value, 
                totalRefundAmount, 
                $"Refund for order {order.ReceiptNo}"
            );
        }

        // 3. Update Status
        bool isFullRefund = refundItems.Sum(x => x.QuantityToRefund) == order.OrderItems.Sum(x => x.Quantity);
        order.OrderStatus = isFullRefund ? RhyoliteOrbitConsts.OrderStatus.Refunded : RhyoliteOrbitConsts.OrderStatus.PartiallyRefunded;
        order.PaymentStatus = isFullRefund ? RhyoliteOrbitConsts.PaymentStatus.Refunded : RhyoliteOrbitConsts.PaymentStatus.PartiallyPaid;

        await _orderRepository.UpdateAsync(order);
    }

    #endregion

    #region Discounts & Queries

    public async Task<PosOrder> ApplyDiscountAsync(Guid orderId, string couponIdentifier)
    {
        var order = await GetOrderByIdAsync(orderId);

        if (order.PaymentStatus == RhyoliteOrbitConsts.PaymentStatus.Completed)
        {
            throw new UserFriendlyException(400, "Cannot apply coupons to paid orders.");
        }

        if (string.IsNullOrWhiteSpace(couponIdentifier))
        {
            throw new UserFriendlyException(400, "Coupon identifier is required.");
        }

        // 1. Fetch Coupon
        var coupon = await _couponRepository.FirstOrDefaultAsync(x => x.CouponIdentifier == couponIdentifier);
        if (coupon == null)
        {
            throw new UserFriendlyException(400, "Invalid or unrecognized coupon identifier.");
        }

        // 2. Validate Coupon Status & Expiry Date
        if (coupon.Status != 1) // Assuming 1 = Active
        {
            throw new UserFriendlyException(400, "Coupon is not active.");
        }

        if (coupon.ValidTill < DateTime.UtcNow)
        {
            throw new UserFriendlyException(400, "Coupon has expired.");
        }

        // 3. Validate Global Usage Quota
        if (coupon.UsageQuota > 0 && coupon.UsageCount >= coupon.UsageQuota)
        {
            throw new UserFriendlyException(400, "Coupon maximum overall usage quota has been reached.");
        }

        // 4. Validate Branch Restriction
        if (coupon.BranchId != Guid.Empty && coupon.BranchId != order.BranchId)
        {
            throw new UserFriendlyException(400, "This coupon is not valid at the selected branch.");
        }

        // 5. Validate Minimum & Maximum Order Subtotal Limits
        if (coupon.MinimumOrderLimit > 0 && order.OrderSubtotal < coupon.MinimumOrderLimit)
        {
            throw new UserFriendlyException(400, $"Order subtotal must be at least {coupon.MinimumOrderLimit:C} to apply this coupon.");
        }

        if (coupon.MaximumOrderLimit > 0 && order.OrderSubtotal > coupon.MaximumOrderLimit)
        {
            throw new UserFriendlyException(400, $"Order subtotal exceeds the maximum limit of {coupon.MaximumOrderLimit:C} allowed for this coupon.");
        }

        // 6. Validate Customer Assignments (if coupon is restricted to specific customers)
        CouponAssignment userAssignment = null;
        if (coupon.Assignments != null && coupon.Assignments.Any())
        {
            if (!order.CustomerId.HasValue)
            {
                throw new UserFriendlyException(400, "A registered customer account is required to use this coupon.");
            }

            userAssignment = coupon.Assignments.FirstOrDefault(x => x.CustomerId == order.CustomerId.Value);
            if (userAssignment == null)
            {
                throw new UserFriendlyException(400, "This coupon is not assigned to the selected customer.");
            }
        }

        // 7. Calculate Discount (Percentage vs Fixed)
        decimal calculatedDiscount = 0;

        if (coupon.UsePercentage)
        {
            // For percentage coupons, CouponValue acts as a percentage (e.g., 15 = 15%)
            calculatedDiscount = (order.OrderSubtotal * coupon.CouponValue) / 100m;
        }
        else
        {
            calculatedDiscount = coupon.CouponValue;
        }

        // Cap discount to prevent negative order totals
        if (calculatedDiscount > order.OrderSubtotal)
        {
            calculatedDiscount = order.OrderSubtotal;
        }

        // 8. Apply Discount to Order
        order.OrderDiscount = calculatedDiscount;
        order.OrderTotal = (order.OrderSubtotal + order.OrderTax + order.ShippingCost) - order.OrderDiscount;
        if (order.OrderTotal < 0) order.OrderTotal = 0;

        await _orderRepository.UpdateAsync(order);

        // 9. Increment Coupon Counters
        coupon.UsageCount += 1;
        if (userAssignment != null)
        {
            userAssignment.UsageCount += 1;
        }

        await _couponRepository.UpdateAsync(coupon);

        return order;
    }

    public async Task<List<PosOrder>> GetOrdersByCustomerAsync(Guid customerId)
    {
        return await _orderRepository.GetAllListAsync(x => x.CustomerId == customerId);
    }

    #endregion

    #region Private Helpers

    private async Task<PosOrder> GetOrderByIdAsync(Guid orderId)
    {
        var order = await _orderRepository.FirstOrDefaultAsync(orderId);
        if (order == null)
        {
            throw new UserFriendlyException(404, "POS Order not found.");
        }
        return order;
    }

    #endregion
}