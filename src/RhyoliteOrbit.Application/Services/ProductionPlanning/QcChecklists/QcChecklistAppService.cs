using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.ProductionPlanning.QcChecklists;
using RhyoliteOrbit.Models.ProductionPlanning;
using RhyoliteOrbit.Services.ProductionPlanning.QcChecklists.Dto;

namespace RhyoliteOrbit.Services.ProductionPlanning.QcChecklists;

public class QcChecklistAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IQcChecklistManager _qcChecklistManager;
    private readonly IMapper _mapper;

    public QcChecklistAppService(IQcChecklistManager qcChecklistManager, IMapper mapper)
    {
        _qcChecklistManager = qcChecklistManager;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _qcChecklistManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task Create(CreateQcChecklistInput input)
    {
        var obj = _mapper.Map<QcChecklist>(input);
        await _qcChecklistManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateQcChecklistInput input)
    {
        var obj = _mapper.Map<QcChecklist>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _qcChecklistManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _qcChecklistManager.Delete(id);
    }
}