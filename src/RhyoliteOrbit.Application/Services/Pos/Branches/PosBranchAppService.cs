using System;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.UI;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.Pos.Branches;
using RhyoliteOrbit.Models.Pos;
using RhyoliteOrbit.Services.Pos.Branches.Dto;

namespace RhyoliteOrbit.Services.Pos.Branches;

[AbpAuthorize]
public class PosBranchAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IPosBranchManager _posBranchManager;
    private readonly IMapper _mapper;

    public PosBranchAppService(IPosBranchManager posBranchManager, IMapper mapper)
    {
        _posBranchManager = posBranchManager;
        _mapper = mapper;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageNo"></param>
    /// <param name="pageSize"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    [AbpAuthorize(PermissionNames.POS_ViewBranches)]
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _posBranchManager.GetAll(pageNo, pageSize, query);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [AbpAuthorize(PermissionNames.POS_CreateBranches)]
    [HttpPost]
    public async Task Create(PosBranchDto dto)
    {
        var obj = _mapper.Map<PosBranch>(dto);
        var tenantInfo = await GetCurrentTenantAsync();
        await _posBranchManager.Create(obj, tenantInfo.DataSupportBusinessAccountId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="id"></param>
    /// <exception cref="UserFriendlyException"></exception>
    [AbpAuthorize(PermissionNames.POS_UpdateBranches)]
    [HttpPut("{id}")]
    public async Task Update(PosBranchDto dto, Guid id)
    {
        var tenantInfo = await GetCurrentTenantAsync();
         
        var obj = _mapper.Map<PosBranch>(dto);
        obj.TenantId = tenantInfo.Id;
        await _posBranchManager.Update(obj, tenantInfo.DataSupportBusinessAccountId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    
    [HttpGet]
    public async Task SetDefaultBranch(Guid id)
    {
        await _posBranchManager.SetDefaultBranch(id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    [AbpAuthorize(PermissionNames.POS_DeleteBranches)]
    [HttpDelete]
    public async Task Delete(Guid id)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        
        await _posBranchManager.Delete(id, tenantInfo.DataSupportBusinessAccountId);
    }
}