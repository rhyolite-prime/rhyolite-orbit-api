using System;
using System.Threading.Tasks;
using Abp.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Pos.BannerSliders;
using RhyoliteOrbit.Models.Pos;
using RhyoliteOrbit.Services.Pos.BannerSliders.Dto;

namespace RhyoliteOrbit.Services.Pos.BannerSliders;

[AbpAuthorize]
public class BannerSliderAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IBannerSliderManager _bannerSliderManager;
    private readonly IMapper _mapper;
    
    public BannerSliderAppService(IBannerSliderManager bannerSliderManager, IMapper mapper)
    {
        _bannerSliderManager = bannerSliderManager;
        _mapper = mapper;
    }
    
    /// <summary>
    /// Get all BannerSlider
    /// </summary>
    /// <param name="pageNo"></param>
    /// <param name="pageSize"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _bannerSliderManager.GetAll(pageNo, pageSize, query);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task Create(BannerSliderDto dto)
    {
        var obj = _mapper.Map<BannerSlider>(dto);
        var tenantInfo = await GetCurrentTenantAsync();
        await _bannerSliderManager.Create(obj, tenantInfo.DataSupportBusinessAccountId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="id"></param>
    [HttpPut("{id}")]
    public async Task Update([FromBody]BannerSliderDto dto, [FromRoute]Guid id)
    {
        var obj = _mapper.Map<BannerSlider>(dto);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        obj.Id = id;
        await _bannerSliderManager.Update(obj, tenantInfo.DataSupportBusinessAccountId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    [HttpDelete]
    public async Task Delete(Guid id)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        await _bannerSliderManager.Delete(id, tenantInfo.DataSupportBusinessAccountId);
    }
}