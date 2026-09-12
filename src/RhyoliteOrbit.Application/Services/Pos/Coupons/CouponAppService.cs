using System;
using System.Threading.Tasks;
using Abp.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.Pos.Coupons;
using RhyoliteOrbit.Models.Pos;
using RhyoliteOrbit.Services.Pos.Coupons.Dto;

namespace RhyoliteOrbit.Services.Pos.Coupons;

[AbpAuthorize]
public class CouponAppService : RhyoliteOrbitAppServiceBase
{
    private readonly ICouponManager _couponManager;
    private readonly IMapper _mapper;

    public CouponAppService(ICouponManager couponManager, IMapper mapper)
    {
        _couponManager = couponManager;
        _mapper = mapper;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageNo"></param>
    /// <param name="pageSize"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    [AbpAuthorize(PermissionNames.POS_ViewCoupons)]
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _couponManager.GetAll(pageNo, pageSize, query);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [AbpAuthorize(PermissionNames.POS_CreateCoupons)]
    [HttpPost]
    public async Task Create(CouponDto dto)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        
        var obj = _mapper.Map<Coupon>(dto);
        await _couponManager.Create(obj, tenantInfo.DataSupportBusinessAccountId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="id"></param>
    [AbpAuthorize(PermissionNames.POS_UpdateCoupons)]
    [HttpPut("{id}")]
    public async Task Update(CouponDto dto, [FromRoute] Guid id)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        
        var obj = _mapper.Map<Coupon>(dto);
        obj.TenantId = tenantInfo.Id;
        obj.Id = id;
        await _couponManager.Update(obj, tenantInfo.DataSupportBusinessAccountId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    [AbpAuthorize(PermissionNames.POS_DeleteCoupons)]
    [HttpDelete]
    public async Task Delete(Guid id)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        await _couponManager.Delete(id, tenantInfo.DataSupportBusinessAccountId);
    }
}