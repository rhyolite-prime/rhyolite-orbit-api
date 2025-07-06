using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.OvertimeTypes;
using RhyoliteOrbit.DomainServices.Payroll.OvertimeTypes.Dto;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.OvertimeTypes.Dto;

namespace RhyoliteOrbit.Services.Payroll.OvertimeTypes;

public class OvertimeTypeAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IOverTimeTypeManager _overTimeTypeManager;
    private readonly IMapper _mapper;

    public OvertimeTypeAppService(IOverTimeTypeManager overTimeTypeManager, IMapper mapper)
    {
        _overTimeTypeManager = overTimeTypeManager;
        _mapper = mapper;
    }

    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _overTimeTypeManager.ListAll();
    }

    [HttpPost]
    public async Task<object> Create(CreateOvertimeTypeInput input)
    {
        var obj = _mapper.Map<OvertimeType>(input);
        return await _overTimeTypeManager.Create(obj);
    }

       
    [HttpPost]
    public async Task<object> CreateOvertimeRate(OvertimeRateDto input)
    {
        return await _overTimeTypeManager.CreateOvertimeRate(input);
    }

    [HttpPost]
    public async Task Update(UpdateOvertimeTypeInput input)
    {
        var obj = _mapper.Map<OvertimeType>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _overTimeTypeManager.Update(obj);
    }

    [HttpPost]
    public async Task UpdateOvertimeRate(OvertimeRateDto input)
    {
        await _overTimeTypeManager.UpdateOvertimeRate(input);
    }
    
    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _overTimeTypeManager.Delete(id);
    }

    [HttpDelete]
    public async Task DeleteOvertimeRate(OvertimeRateDto input)
    {
        await _overTimeTypeManager.DeleteOvertimeRate(input);
    }
}