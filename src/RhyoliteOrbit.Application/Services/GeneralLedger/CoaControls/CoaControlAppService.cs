using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.GeneralLedger.CoaControls;
using RhyoliteOrbit.Models.GeneralLedger;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Services.GeneralLedger.CoaControls.Dto;
using RhyoliteOrbit.Services.SchoolManager.BillTypes.Dto;

namespace RhyoliteOrbit.Services.GeneralLedger.CoaControls;

public class CoaControlAppService: RhyoliteOrbitAppServiceBase
{
    private readonly ICoaControlManager _coaControlManager;
    private readonly IMapper _mapper;

    public CoaControlAppService(ICoaControlManager coaControlManager , IMapper mapper)
    {
        _coaControlManager = coaControlManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query,  Guid accountHeadId)
    {
        return await _coaControlManager.ListAll(pageNo, pageSize, query, accountHeadId);
    }
    
    [HttpGet]
    public async Task<object> GetControlsByAccountHead(Guid id)
    {
        return await _coaControlManager.ListAll(id);
    }

    [HttpPost]
    public async Task Create(CreateCoaControlInput input)
    {
        var obj = _mapper.Map<CoaControl>(input);
        await _coaControlManager.Create(obj);
    }

    public async Task<object> GetAllByAccountGroup(Guid id)
    {
        return await _coaControlManager.ListAll(id);
    }
    
    [HttpPost]
    public async Task Update(UpdateCoaControlInput input)
    {
        var obj = _mapper.Map<CoaControl>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _coaControlManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _coaControlManager.Delete(id);

    }
}