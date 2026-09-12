using System;
using System.Threading.Tasks;
using Abp.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.Pos.PosInventoryItems;
using RhyoliteOrbit.Models.Pos;
using RhyoliteOrbit.Services.Pos.Inventory.Dto;

namespace RhyoliteOrbit.Services.Pos.Inventory;

[AbpAuthorize]
public class PosInventoryAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IPosInventoryItemManager _posInventoryItemManager;
    private readonly IMapper _mapper;
    
    public PosInventoryAppService(IPosInventoryItemManager posInventoryItemManager, IMapper mapper)
    {
        _posInventoryItemManager = posInventoryItemManager;
        _mapper = mapper;
    }


    /// <summary>∂
    /// 
    /// </summary>
    /// <param name="pageNo"></param>
    /// <param name="pageSize"></param>
    /// <param name="query"></param>
    /// <param name="shelfLocation"></param>
    /// <param name="branchId"></param>
    /// <param name="categoryId"></param>
    /// <param name="merchantId"></param>
    /// <param name="itemType"></param>
    /// <returns></returns>
    [AbpAuthorize(PermissionNames.POS_ViewProductsAndServices)]
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query, string shelfLocation, string branchId, string categoryId, string merchantId,string itemType)
    {
        return await _posInventoryItemManager.GetAll(pageNo, pageSize, query, shelfLocation, branchId, categoryId, merchantId, itemType);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [AbpAuthorize(PermissionNames.POS_CreateProductsAndServices)]
    [HttpPost]
    public async Task Create(PosInventoryItemDto dto)
    {
        var obj = _mapper.Map<PosInventoryItem>(dto);
        var tenantInfo = await GetCurrentTenantAsync();
        await _posInventoryItemManager.Create(obj, tenantInfo.DataSupportBusinessAccountId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="id"></param>
    [AbpAuthorize(PermissionNames.POS_UpdateProductsAndServices)]
    [HttpPut("{id}")]
    public async Task Update(PosInventoryItemDto dto, [FromRoute]Guid id)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        
        var obj = _mapper.Map<PosInventoryItem>(dto);
        
        obj.TenantId = tenantInfo.Id;
        obj.Id = id;
        await _posInventoryItemManager.Update(obj, tenantInfo.DataSupportBusinessAccountId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    [AbpAuthorize(PermissionNames.POS_DeleteProductsAndServices)]
    [HttpDelete]
    public async Task Delete(Guid id)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        
        await _posInventoryItemManager.Delete(id, tenantInfo.DataSupportBusinessAccountId);
    }

    [AbpAuthorize(PermissionNames.POS_UpdateProductsAndServices)]
    [HttpPut]
    public async Task UpdateProductSpecification(ProductSpecificationDto dto)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        await _posInventoryItemManager.UpdateProductSpecification(dto.Id, dto.Specifications, tenantInfo.DataSupportBusinessAccountId);
    }
    
    [AbpAuthorize(PermissionNames.POS_UpdateProductsAndServices)]
    [HttpPost]
    public async Task AddProductAttributes(ProductAttributeDto dto)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        await _posInventoryItemManager.AddProductAttributes(dto.ItemId, dto.Attributes, tenantInfo.DataSupportBusinessAccountId);
    }
    
    [AbpAuthorize(PermissionNames.POS_UpdateProductsAndServices)]
    [HttpPut]
    public async Task UpdateProductAttributes(ProductAttributeDto dto)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        await _posInventoryItemManager.UpdateProductAttributes(dto.ItemId, dto.Attributes, tenantInfo.DataSupportBusinessAccountId);
    }
    
     
    [HttpGet]
    public async Task GetDetails(Guid id)
    {
        await _posInventoryItemManager.GetDetails(id);
    }
    
}