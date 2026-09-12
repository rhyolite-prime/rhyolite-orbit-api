using System;
using System.Threading.Tasks;
using Abp.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.Pos.InventoryCategories;
using RhyoliteOrbit.Models.Pos;
using RhyoliteOrbit.Services.Pos.Categories.Dto;

namespace RhyoliteOrbit.Services.Pos.Categories;

[AbpAuthorize]
public class PosInventoryCategoryAppService : RhyoliteOrbitAppServiceBase
{
 
    private readonly IInventoryCategoryManager _inventoryCategoryManager;
    private readonly IMapper _mapper;
    

    public PosInventoryCategoryAppService(IInventoryCategoryManager categoryManager, IMapper mapper)
    {
        _inventoryCategoryManager = categoryManager;
        _mapper = mapper;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageNo"></param>
    /// <param name="pageSize"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    [AbpAuthorize(PermissionNames.POS_ViewCategories)]
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _inventoryCategoryManager.GetAll(pageNo, pageSize, query);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [AbpAuthorize(PermissionNames.POS_CreateCategories)]
    [HttpPost]
    public async Task Create(PosInventoryCategoryDto dto)
    {
        var obj = _mapper.Map<InventoryCategory>(dto);
        var tenantInfo = await GetCurrentTenantAsync();
        await _inventoryCategoryManager.Create(obj, tenantInfo.DataSupportBusinessAccountId);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="id"></param>
    [AbpAuthorize(PermissionNames.POS_UpdateCategories)]
    [HttpPut("{id}")]
    public async Task Update(PosInventoryCategoryDto dto, [FromRoute]Guid id)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        
        var obj = _mapper.Map<InventoryCategory>(dto);
        obj.TenantId = tenantInfo.Id;
        obj.Id = id;
        await _inventoryCategoryManager.Update(obj, tenantInfo.DataSupportBusinessAccountId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    [AbpAuthorize(PermissionNames.POS_DeleteCategories)]
    [HttpDelete]
    public async Task Delete(Guid id)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        await _inventoryCategoryManager.Delete(id, tenantInfo.DataSupportBusinessAccountId);
    }
}