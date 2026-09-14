using System;
using System.Threading.Tasks;
using Abp.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.Pos.CustomerRanks;
using RhyoliteOrbit.Models.Pos;
using RhyoliteOrbit.Services.Pos.CustomerRanks.Dto;
using RhyoliteOrbit.Services.Pos.Customers.Dto;

namespace RhyoliteOrbit.Services.Pos.CustomerRanks;

[AbpAuthorize]
public class CustomerRankAppService : RhyoliteOrbitAppServiceBase
{
    private readonly ICustomerRankManager _customerRankManager;
    private readonly IMapper _mapper;

    public CustomerRankAppService(ICustomerRankManager customerRankManager, IMapper mapper)
    {
        _customerRankManager = customerRankManager;
        _mapper = mapper;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageNo"></param>
    /// <param name="pageSize"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    [AbpAuthorize(PermissionNames.POS_ViewCustomerRanks)]
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _customerRankManager.GetAll(pageNo, pageSize, query);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [AbpAuthorize(PermissionNames.POS_CreateCustomerRanks)]
    [HttpPost]
    public async Task Create(CustomerRankDto dto)
    {
        var obj = _mapper.Map<CustomerRank>(dto);
        await _customerRankManager.Create(obj);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="id"></param>
    [AbpAuthorize(PermissionNames.POS_UpdateCustomerRanks)]
    [HttpPut("{id}")]
    public async Task Update(PosCustomerDto dto, Guid id)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        
        var obj = _mapper.Map<CustomerRank>(dto);
        obj.TenantId = tenantInfo.Id;
        obj.Id = id;
        await _customerRankManager.Update(obj);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    [AbpAuthorize(PermissionNames.POS_DeleteCustomerRanks)]
    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _customerRankManager.Delete(id);
    }
    
}