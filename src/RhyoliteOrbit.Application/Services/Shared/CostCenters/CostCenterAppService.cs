using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Shared.CostCenters;
using RhyoliteOrbit.Models.Shared;
using RhyoliteOrbit.Services.Shared.CostCenters.Dto;
using RhyoliteOrbit.Services.Shared.Currencies.Dto;


namespace RhyoliteOrbit.Services.Shared.CostCenters;

public class CostCenterAppService: RhyoliteOrbitAppServiceBase
{
    private readonly ICostCenterManager _costCenterManager;
    private readonly IMapper _mapper;

    public CostCenterAppService(ICostCenterManager currencyManager, IMapper mapper)
    {
        _costCenterManager = currencyManager;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _costCenterManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task Create(CreateCostCenterInput input)
    {
        var obj = _mapper.Map<CostCenter>(input);
        await _costCenterManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateCostCenterInput input)
    {
        var obj = _mapper.Map<CostCenter>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _costCenterManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _costCenterManager.Delete(id);
    }
}