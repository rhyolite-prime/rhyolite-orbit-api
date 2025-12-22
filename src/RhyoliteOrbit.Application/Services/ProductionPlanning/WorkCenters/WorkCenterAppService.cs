using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.ProductionPlanning.WorkCenters;
using RhyoliteOrbit.Models.ProductionPlanning;
using RhyoliteOrbit.Services.ProductionPlanning.RawMaterials.Dto;
using RhyoliteOrbit.Services.ProductionPlanning.WorkCenters.Dto;

namespace RhyoliteOrbit.Services.ProductionPlanning.WorkCenters;

public class WorkCenterAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IWorkCenterManager  _workCenterManager;
    private readonly IMapper _mapper;

    public WorkCenterAppService(IWorkCenterManager workCenterManager, IMapper mapper)
    {
        _workCenterManager = workCenterManager;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _workCenterManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task Create(CreateWorkCenterInput input)
    {
        var obj = _mapper.Map<WorkCenter>(input);
        await _workCenterManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateWorkCenterInput input)
    {
        var obj = _mapper.Map<WorkCenter>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _workCenterManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _workCenterManager.Delete(id);
    }
}