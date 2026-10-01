using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.Wishlists;

public class WishlistManager: DomainService, IWishlistManager
{
    private readonly IRepository<Wishlist, Guid> _wishlistRepository;
    private readonly IRepository<PosInventoryItem, Guid> _posInventoryItemRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public WishlistManager(IRepository<Wishlist, Guid> wishlistRepository, IRepository<PosInventoryItem, Guid> posInventoryItemRepository, IUnitOfWorkManager unitOfWorkManager)
    {
        _wishlistRepository = wishlistRepository;
        _posInventoryItemRepository = posInventoryItemRepository;
        _unitOfWorkManager = unitOfWorkManager;
    }
    
    public async Task AddToWishlist(int tenantId, Guid customerId, Guid itemId)
    {
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            
            var itemInfo = await _posInventoryItemRepository.FirstOrDefaultAsync(itemId);

            if (itemInfo != null)
            {
                // Check if item already exists in cart
                var existingCartItem = await _wishlistRepository.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.ItemId == itemId);

                var newShopCart = new Wishlist
                {
                    CustomerId = customerId,
                    ItemId = itemId,
                    ItemName = itemInfo.Name,
                    Sku = itemInfo.Sku,
                    UnitPrice = itemInfo.SellingPrice,
                    MinOrderQuantity = itemInfo.MinimumCartQuantity,
                    ItemImageUrl = itemInfo.Images.FirstOrDefault(), 
                    TenantId = tenantId
                };

                await _wishlistRepository.InsertAsync(newShopCart);
                
            }
            
        }
    }

    
    public async Task RemoveItem(int tenantId, Guid customerId, Guid cartItemId)
    {
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            var shopCartRow = await _wishlistRepository.FirstOrDefaultAsync(x=> x.ItemId == cartItemId && x.CustomerId == customerId);
            await _wishlistRepository.DeleteAsync(shopCartRow);   
        }
    }

    public async Task Clear(int tenantId, Guid customerId)
    {
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            await _wishlistRepository.DeleteAsync(x => x.CustomerId == customerId);   
        }
    }
    
    
    public async Task<object> GetWishlistItems(int tenantId, Guid customerId)
    {
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            return await (await _wishlistRepository.GetAllAsync())
                .Where(x => x.CustomerId == customerId)
                .Select(x => new
                {
                    x.Id,
                    x.ItemId,
                    x.Sku,
                    x.UnitPrice,
                    x.ItemName,
                    x.MinOrderQuantity,
                    x.ItemImageUrl
                })
                .ToListAsync();
        }
    }

    public async Task<int> GetWishlistItemCount(int tenantId, Guid customerId)
    {
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            return await _wishlistRepository.CountAsync(x => x.CustomerId == customerId);
        }

    }

    
    
    
}