using System;
using System.Threading.Tasks;
using Abp.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.Pos.DeliveryChannels;
using RhyoliteOrbit.Models.Pos;
using RhyoliteOrbit.Services.Pos.DeliveryChannels.Dto;

namespace RhyoliteOrbit.Services.Pos.DeliveryChannels;

[AbpAuthorize]
public class DeliveryChannelAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IDeliveryChannelManager _deliveryChannelManager;
    private readonly IMapper _mapper;

    public DeliveryChannelAppService(IDeliveryChannelManager deliveryChannelManager, IMapper mapper)
    {
        _deliveryChannelManager = deliveryChannelManager;
        _mapper = mapper;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageNo"></param>
    /// <param name="pageSize"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    [AbpAuthorize(PermissionNames.POS_ViewDeliveryChannels)]
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _deliveryChannelManager.GetAll(pageNo, pageSize, query);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [AbpAuthorize(PermissionNames.POS_CreateDeliveryChannel)]
    [HttpPost]
    public async Task Create(DeliveryChannelDto dto)
    {
        var obj = _mapper.Map<DeliveryChannel>(dto);
        var tenantInfo = await GetCurrentTenantAsync();
        await _deliveryChannelManager.Create(obj, tenantInfo.DataSupportBusinessAccountId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="id"></param>
    [AbpAuthorize(PermissionNames.POS_UpdateDeliveryChannel)]
    [HttpPut("{id}")]
    public async Task Update(DeliveryChannelDto dto, Guid id)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        
        var obj = _mapper.Map<DeliveryChannel>(dto);
        obj.Id = id;
        obj.TenantId = tenantInfo.Id;
        await _deliveryChannelManager.Update(obj, tenantInfo.DataSupportBusinessAccountId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    [AbpAuthorize(PermissionNames.POS_DeleteDeliveryChannel)]
    [HttpDelete]
    public async Task Delete(Guid id)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        await _deliveryChannelManager.Delete(id, tenantInfo.DataSupportBusinessAccountId);
    }
}