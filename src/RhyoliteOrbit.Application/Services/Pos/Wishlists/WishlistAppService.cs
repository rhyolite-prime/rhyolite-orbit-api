using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.Pos.Wishlists;

namespace RhyoliteOrbit.Services.Pos.Wishlists;

public class WishlistAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IWishlistManager _wishlistManager;
    private readonly IValidateAuth _validateAuth;

    public WishlistAppService(IWishlistManager wishlistManager, IValidateAuth validateAuth)
    {
        _wishlistManager = wishlistManager;
        _validateAuth = validateAuth;
    }
    
    [HttpGet]
    public async Task AddToWishlist([FromQuery]Guid itemId)
    {
        var tenantId = await _validateAuth.ValidateTenant();
        var customerId = await _validateAuth.ValidateCustomer();

        await _wishlistManager.AddToWishlist(tenantId,customerId, itemId);
    }
    
    [HttpGet]
    public async Task RemoveItem([FromQuery]Guid itemId)
    {
        var tenantId = await _validateAuth.ValidateTenant();
        var customerId = await _validateAuth.ValidateCustomer();

        await _wishlistManager.RemoveItem(tenantId, customerId, itemId);
    }
    
    [HttpGet]
    public async Task Clear()
    {
        var tenantId = await _validateAuth.ValidateTenant();
        var customerId = await _validateAuth.ValidateCustomer();
        
        await _wishlistManager.Clear(tenantId, customerId);
    }
    
    [HttpGet]
    public async Task<object> GetWishlistItems()
    {
        var tenantId = await _validateAuth.ValidateTenant();
        var customerId = await _validateAuth.ValidateCustomer();
        
        return await _wishlistManager.GetWishlistItems(tenantId, customerId);
    }

    [HttpGet]
    public async Task<int> GetWishlistItemCount()
    {
        var tenantId = await _validateAuth.ValidateTenant();
        var customerId = await _validateAuth.ValidateCustomer();
        
        return await _wishlistManager.GetWishlistItemCount(tenantId, customerId);
    }
}