using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Shared.UnitOfMeasurements;
using RhyoliteOrbit.Models.Shared;
using RhyoliteOrbit.Services.Shared.UnitOfMeasurements.Dto;

namespace RhyoliteOrbit.Services.Shared.UnitOfMeasurements;

public class UnitOfMeasureAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IUnitOfMeasureManager _rawMaterialManager;
    private readonly IMapper _mapper;

    public UnitOfMeasureAppService(IUnitOfMeasureManager rawMaterialManager, IMapper mapper)
    {
        _rawMaterialManager = rawMaterialManager;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _rawMaterialManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task Create(CreateUnitOfMeasureInput input)
    {
        var obj = _mapper.Map<UnitOfMeasure>(input);
        await _rawMaterialManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateUnitOfMeasureInput input)
    {
        var obj = _mapper.Map<UnitOfMeasure>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _rawMaterialManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _rawMaterialManager.Delete(id);
    }
    
}