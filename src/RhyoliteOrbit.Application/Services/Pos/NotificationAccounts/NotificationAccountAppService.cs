using System;
using System.Threading.Tasks;
using Abp.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.Pos.NotificationAccounts;
using RhyoliteOrbit.Models.Pos;
using RhyoliteOrbit.Services.Pos.NotificationAccounts.Dto;

namespace RhyoliteOrbit.Services.Pos.NotificationAccounts;

[AbpAuthorize]
public class NotificationAccountAppService: RhyoliteOrbitAppServiceBase
{
    private readonly INotificationAccountManager _notificationAccountManager;
    private readonly IMapper _mapper;

    public NotificationAccountAppService(INotificationAccountManager notificationAccountManager, IMapper mapper)
    {
        _notificationAccountManager = notificationAccountManager;
        _mapper = mapper;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageNo"></param>
    /// <param name="pageSize"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    [AbpAuthorize(PermissionNames.POS_ViewNotificationAccounts)]
    [HttpGet]
    public async Task<object> GetAll()
    {
        var tenantInfo = await GetCurrentTenantAsync();
        
        return await _notificationAccountManager.GetAll(tenantInfo.Id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [AbpAuthorize(PermissionNames.POS_CreateNotificationAccount)]
    [HttpPost]
    public async Task Create(NotificationAccountDto dto)
    {
        var obj = _mapper.Map<NotificationAccount>(dto);
        await _notificationAccountManager.Create(obj);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="id"></param>
    [AbpAuthorize(PermissionNames.POS_UpdateNotificationAccount)]
    [HttpPost]
    public async Task Update(NotificationAccountDto dto, Guid id)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        
        var obj = _mapper.Map<NotificationAccount>(dto);
        obj.TenantId = tenantInfo.Id;
        obj.Id = id;
        await _notificationAccountManager.Update(obj);
    }
     

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    [AbpAuthorize(PermissionNames.POS_DeleteCreateNotificationAccount)]
    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _notificationAccountManager.Delete(id);
    }
    
    
     
    
}