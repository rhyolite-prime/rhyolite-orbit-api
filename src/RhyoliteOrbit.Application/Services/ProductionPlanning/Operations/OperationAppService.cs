using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.ProductionPlanning.JobTickets;
using RhyoliteOrbit.Models.ProductionPlanning;
using RhyoliteOrbit.Services.ProductionPlanning.JobTickets.Dto;

namespace RhyoliteOrbit.Services.ProductionPlanning.JobTickets;

public class OperationAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IOperationManager _operationManager;
    private readonly IMapper _mapper;

    public OperationAppService(IOperationManager operationManager, IMapper mapper)
    {
        _operationManager = operationManager;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _operationManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task Create(CreateOperationInput input)
    {
        var obj = _mapper.Map<Operation>(input);
        await _operationManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateOperationInput input)
    {
        var obj = _mapper.Map<Operation>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _operationManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _operationManager.Delete(id);
    }
}