using System;
using System.Threading.Tasks;
using Abp.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.Pos.ShopCarts;
using RhyoliteOrbit.Services.Pos.ShopCarts.Dto;

namespace RhyoliteOrbit.Services.Pos.ShopCarts;

public class ShopCartAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IShopCartManager _shopCartManager;
    private readonly IValidateAuth _validateAuth;

    public ShopCartAppService(IShopCartManager shopCartManager, IValidateAuth validateAuth)
    {
        _shopCartManager = shopCartManager;
        _validateAuth = validateAuth;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    [HttpPost]
    public async Task AddToCart(ShopCartDto dto)
    {
        var tenantId = await _validateAuth.ValidateTenant();
        var customerId = await _validateAuth.ValidateCustomer();

        await _shopCartManager.AddToCart(tenantId,customerId, dto.ItemId, dto.Quantity);
    }

    [HttpPost]
    public async Task UpdateQuantity(ShopCartDto dto)
    {
        var tenantId = await _validateAuth.ValidateTenant();
        var customerId = await _validateAuth.ValidateCustomer();
        
        await _shopCartManager.UpdateQuantity(tenantId, customerId, dto.ItemId, dto.Quantity);
    }

    [HttpGet]
    public async Task RemoveItem([FromQuery]Guid itemId)
    {
        var tenantId = await _validateAuth.ValidateTenant();
        var customerId = await _validateAuth.ValidateCustomer();

        await _shopCartManager.RemoveItem(tenantId, customerId, itemId);
    }

    [HttpGet]
    public async Task ClearCart()
    {
        var tenantId = await _validateAuth.ValidateTenant();
        var customerId = await _validateAuth.ValidateCustomer();
        
        await _shopCartManager.ClearCart(tenantId, customerId);
    }

    [HttpGet]
    public async Task<object> GetCartItems()
    {
        var tenantId = await _validateAuth.ValidateTenant();
        var customerId = await _validateAuth.ValidateCustomer();
        
       return await _shopCartManager.GetCartItems(tenantId, customerId);
    }

    [HttpGet]
    public async Task<int> GetCartItemCount()
    {
        var tenantId = await _validateAuth.ValidateTenant();
        var customerId = await _validateAuth.ValidateCustomer();
        
        return await _shopCartManager.GetCartItemCount(tenantId, customerId);
    }

    [HttpGet]
    public async Task<decimal> GetCartSubtotal()
    {
        var tenantId = await _validateAuth.ValidateTenant();
        var customerId = await _validateAuth.ValidateCustomer();
        
        return await _shopCartManager.GetCartSubtotal(tenantId, customerId);
        
    }
    
    [HttpGet]
    public async Task MergeGuestCart(string guestUsername)
    {
        var tenantId = await _validateAuth.ValidateTenant();
        var customerId = await _validateAuth.ValidateCustomer();
        
        await _shopCartManager.MergeGuestCart(tenantId, guestUsername, customerId);
    }
    
}