using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.ProductionPlanning.ProcessFlows;
using RhyoliteOrbit.Models.ProductionPlanning;
using RhyoliteOrbit.Services.ProductionPlanning.ProcessFlows.Dto;

namespace RhyoliteOrbit.Services.ProductionPlanning.ProcessFlows;

public class ProcessFlowAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IProcessFlowManager  _processFlowManager;
    private readonly IMapper _mapper;

    public ProcessFlowAppService(IProcessFlowManager processFlowManager, IMapper mapper)
    {
        _processFlowManager = processFlowManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _processFlowManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task Create(CreateProcessFlowInput input)
    {
        var obj = _mapper.Map<ProcessFlow>(input);
        await _processFlowManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateProcessFlowInput input)
    {
        var obj = _mapper.Map<ProcessFlow>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _processFlowManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _processFlowManager.Delete(id);
    }
}