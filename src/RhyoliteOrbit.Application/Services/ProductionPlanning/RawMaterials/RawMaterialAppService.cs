using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.ProductionPlanning.RawMaterials;
using RhyoliteOrbit.Models.ProductionPlanning;
using RhyoliteOrbit.Services.ProductionPlanning.RawMaterials.Dto;

namespace RhyoliteOrbit.Services.ProductionPlanning.RawMaterials;

public class RawMaterialAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IRawMaterialManager  _rawMaterialManager;
    private readonly IMapper _mapper;

    public RawMaterialAppService(IRawMaterialManager rawMaterialManager, IMapper mapper)
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
    public async Task Create(CreateRawMaterialInput input)
    {
        var obj = _mapper.Map<RawMaterial>(input);
        await _rawMaterialManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateRawMaterialInput input)
    {
        var obj = _mapper.Map<RawMaterial>(input);
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