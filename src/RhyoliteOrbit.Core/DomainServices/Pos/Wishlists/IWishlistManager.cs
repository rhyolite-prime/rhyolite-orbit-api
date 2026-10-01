using System;
using System.Threading.Tasks;
using Abp.Domain.Services;

namespace RhyoliteOrbit.DomainServices.Pos.Wishlists;

public interface IWishlistManager: IDomainService
{
     
    Task AddToWishlist(int tenantId, Guid customerId, Guid itemId);
    Task RemoveItem(int tenantId, Guid customerId, Guid cartItemId);
    Task Clear(int tenantId, Guid customerId);
    Task<object> GetWishlistItems(int tenantId, Guid customerId);
    Task<int> GetWishlistItemCount(int tenantId, Guid customerId);
}