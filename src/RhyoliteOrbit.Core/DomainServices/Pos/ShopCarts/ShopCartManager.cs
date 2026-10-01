using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.ShopCarts;

public class ShopCartManager: DomainService, IShopCartManager
{
    
    private readonly IRepository<ShopCart, Guid> _shopCartRepository;
    private readonly IRepository<PosInventoryItem, Guid> _posInventoryItemRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public ShopCartManager(IRepository<ShopCart, Guid> shopCartRepository, IRepository<PosInventoryItem, Guid> posInventoryItemRepository, IUnitOfWorkManager unitOfWorkManager)
    {
        _shopCartRepository = shopCartRepository;
        _posInventoryItemRepository = posInventoryItemRepository;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task AddToCart(int tenantId, Guid customerId, Guid itemId, int quantity)
    {
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            
            var itemInfo = await _posInventoryItemRepository.FirstOrDefaultAsync(itemId);

            if (itemInfo != null)
            {
                // Check if item already exists in cart
                var existingCartItem = await _shopCartRepository.FirstOrDefaultAsync(
                    x => x.CustomerId == customerId && x.ItemId == itemId
                );

                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += quantity;
            
                    // Keep price synced with latest addition
                    existingCartItem.UnitPrice = itemInfo.SellingPrice; 
                    existingCartItem.TenantId = tenantId; 

                    await _shopCartRepository.UpdateAsync(existingCartItem);
                }
                else
                {

                    var customerInfo = await _shopCartRepository.FirstOrDefaultAsync(x=> x.CustomerId == customerId);
                
                    var newShopCart = new ShopCart
                    {
                        CustomerId = customerId,
                        Username = customerInfo != null ? customerInfo.Username : string.Empty,
                        ItemId = itemId,
                        ItemName = itemInfo.Name,
                        Sku = itemInfo.Sku,
                        Quantity = quantity,
                        UnitPrice = itemInfo.SellingPrice,
                        MinOrderQuantity = itemInfo.MinimumCartQuantity,
                        ItemImageUrl = itemInfo.Images.FirstOrDefault(), 
                        TenantId = tenantId
                    };

                    await _shopCartRepository.InsertAsync(newShopCart);
                }
            }
            
        }
    }

    public async Task UpdateQuantity(int tenantId, Guid customerId, Guid cartItemId, int newQuantity)
    {
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            var cartItem = await _shopCartRepository.FirstOrDefaultAsync(x=> x.CustomerId == customerId && x.ItemId == cartItemId);
            if (cartItem == null)
            {
                throw new UserFriendlyException(404, "Cart item not found.");
            }

            if (newQuantity <= 0)
            {
                await _shopCartRepository.DeleteAsync(cartItem);
                return;
            }

            if (cartItem.MinOrderQuantity > 0 && newQuantity < cartItem.MinOrderQuantity)
            {
                throw new UserFriendlyException(400, $"Minimum order quantity for this item is {cartItem.MinOrderQuantity}.");
            }

            cartItem.Quantity = newQuantity;
            await _shopCartRepository.UpdateAsync(cartItem);
        }
    }

    public async Task RemoveItem(int tenantId, Guid customerId, Guid cartItemId)
    {
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            var shopCartRow = await _shopCartRepository.FirstOrDefaultAsync(x=> x.ItemId == cartItemId && x.CustomerId == customerId);
            await _shopCartRepository.DeleteAsync(shopCartRow);   
        }
    }

    public async Task ClearCart(int tenantId, Guid customerId)
    {
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            await _shopCartRepository.DeleteAsync(x => x.CustomerId == customerId);   
        }
    }
    
    public async Task<object> GetCartItems(int tenantId, Guid customerId)
    {
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            return await (await _shopCartRepository.GetAllAsync())
                .Where(x => x.CustomerId == customerId)
                .Select(x => new
                {
                    x.Id,
                    x.ItemId,
                    x.Sku,
                    x.ItemName,
                    x.Quantity,
                    x.UnitPrice,
                    x.MinOrderQuantity,
                    x.ItemImageUrl
                })
                .ToListAsync();
        }
    }

    public async Task<int> GetCartItemCount(int tenantId, Guid customerId)
    {
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            var itemList = await _shopCartRepository.GetAllListAsync(x => x.CustomerId == customerId);
            return itemList.Sum(x => x.Quantity);
        }

    }

    public async Task<decimal> GetCartSubtotal(int tenantId, Guid customerId)
    {
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            return await (await _shopCartRepository.GetAllAsync())
                .Where(x => x.CustomerId == customerId)
                .SumAsync(x => x.Quantity * x.UnitPrice);   
        }
    }

     
    public async Task MergeGuestCart(int tenantId, string guestUsername, Guid customerId)
    {
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            if (string.IsNullOrWhiteSpace(guestUsername))
            {
                return;
            }

            var guestItems = await _shopCartRepository.GetAllListAsync(x => x.Username == guestUsername);
            if (!guestItems.Any())
            {
                return;
            }

            var customerItems = await _shopCartRepository.GetAllListAsync(x => x.CustomerId == customerId);

            foreach (var guestItem in guestItems)
            {
                var matchingCustomerItem = customerItems.FirstOrDefault(x => x.ItemId == guestItem.ItemId);

                if (matchingCustomerItem != null)
                {
                    // Increment quantity on the user's existing item
                    matchingCustomerItem.Quantity += guestItem.Quantity;
                    await _shopCartRepository.UpdateAsync(matchingCustomerItem);
                
                    // Delete guest record
                    await _shopCartRepository.DeleteAsync(guestItem);
                }
                else
                {
                    // Re-assign guest item to the authenticated customer
                    guestItem.CustomerId = customerId;
                    await _shopCartRepository.UpdateAsync(guestItem);
                }
            }   
        }
    }
    
}