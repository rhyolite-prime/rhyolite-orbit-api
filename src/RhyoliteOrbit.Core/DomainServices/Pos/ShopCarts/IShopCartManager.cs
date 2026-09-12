using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.ShopCarts;

public interface IShopCartManager: IDomainService
{
    // Cart Items
    Task AddToCart(int tenantId, Guid customerId, Guid itemId, int quantity);
    
    Task UpdateQuantity(int tenantId, Guid customerId, Guid cartItemId, int newQuantity);
    
    Task RemoveItem(int tenantId, Guid customerId, Guid cartItemId);
    Task ClearCart(int tenantId, Guid customerId);

    // Queries & Summaries
    Task<object> GetCartItems(int tenantId, Guid customerId);
    Task<int> GetCartItemCount(int tenantId, Guid customerId);
    Task<decimal> GetCartSubtotal(int tenantId, Guid customerId);

    // Guest & Account Operations
    Task MergeGuestCart(int tenantId, string guestUsername, Guid customerId);
}