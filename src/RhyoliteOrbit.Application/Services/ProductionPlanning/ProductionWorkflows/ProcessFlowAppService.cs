using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.ProductionPlanning.ProductionWorkflows;
using RhyoliteOrbit.Services.ProductionPlanning.ProcessFlows.Dto;
using ProductionWorkflow = RhyoliteOrbit.Models.ProductionPlanning.ProductionWorkflow;

namespace RhyoliteOrbit.Services.ProductionPlanning.ProcessFlows;

public class ProcessFlowAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IProductionWorkflow  _productionWorkflow;
    private readonly IMapper _mapper;

    public ProcessFlowAppService(IProductionWorkflow productionWorkflow, IMapper mapper)
    {
        _productionWorkflow = productionWorkflow;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _productionWorkflow.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task Create(CreateProcessFlowInput input)
    {
        var obj = _mapper.Map<ProductionWorkflow>(input);
        await _productionWorkflow.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateProcessFlowInput input)
    {
        var obj = _mapper.Map<ProductionWorkflow>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _productionWorkflow.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _productionWorkflow.Delete(id);
    }
}