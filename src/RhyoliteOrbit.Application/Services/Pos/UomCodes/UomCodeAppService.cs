using System;
using System.Threading.Tasks;
using Abp.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.Pos.UomCodes;
using RhyoliteOrbit.Models.Pos;
using RhyoliteOrbit.Services.Pos.UomCodes.Dto;

namespace RhyoliteOrbit.Services.Pos.UomCodes;

[AbpAuthorize]
public class UomCodeAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IUomCodeManager _uomCodeManager;
    private readonly IMapper _mapper;
    
    public UomCodeAppService(IUomCodeManager uomCodeManager, IMapper mapper)
    {
        _uomCodeManager = uomCodeManager;
        _mapper = mapper;
    }
    
    /// <summary>
    /// Get all UomCodes
    /// </summary>
    /// <param name="pageNo"></param>
    /// <param name="pageSize"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    [AbpAuthorize(PermissionNames.POS_ViewUnitsOfMeasurement)]
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _uomCodeManager.GetAll(pageNo, pageSize, query);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [AbpAuthorize(PermissionNames.POS_CreateUnitsOfMeasurement)]
    [HttpPost]
    public async Task Create(UomCodeDto dto)
    {
        var obj = _mapper.Map<UomCode>(dto);
        await _uomCodeManager.Create(obj);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="id"></param>
    [AbpAuthorize(PermissionNames.POS_UpdateUnitsOfMeasurement)]
    [HttpPut("{id}")]
    public async Task Update(UomCodeDto dto, [FromRoute]Guid id)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        
        var obj = _mapper.Map<UomCode>(dto);
        
        obj.TenantId = tenantInfo.Id;
        obj.Id = id;
        await _uomCodeManager.Update(obj);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    [AbpAuthorize(PermissionNames.POS_DeleteUnitsOfMeasurement)]
    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _uomCodeManager.Delete(id);
    }
}