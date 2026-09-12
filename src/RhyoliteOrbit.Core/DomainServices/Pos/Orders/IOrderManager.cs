using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Pos;


namespace RhyoliteOrbit.DomainServices.Pos.Orders;

public interface IOrderManager: IDomainService
{
    Task<PosOrder> CreateOrderAsync(PosOrder order);
    Task ProcessPaymentAsync(Guid orderId, string paymentMethod, decimal amountTendered, string paymentReference);
    Task CancelOrderAsync(Guid orderId, string reason);
    Task RefundOrderAsync(Guid orderId, List<RefundRequestDto> refundedItemIds, string reason, long processedByUserId);
    Task<PosOrder> ApplyDiscountAsync(Guid orderId, string discountCode);
    Task<List<PosOrder>> GetOrdersByCustomerAsync(Guid customerId);
}