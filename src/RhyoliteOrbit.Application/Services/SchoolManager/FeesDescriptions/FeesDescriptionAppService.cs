using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Services.Payroll.FeesDescriptions.Dto;

namespace RhyoliteOrbit.DomainServices.SchoolManager.FeesDescriptions;

public class FeesDescriptionAppService : RhyoliteOrbitAppServiceBase
{
    private readonly IFeesDescriptionManager _feesDescriptionManager;
    private readonly IMapper _mapper;
    public FeesDescriptionAppService(IFeesDescriptionManager feesDescriptionManager, IMapper mapper)
    {
        _feesDescriptionManager = feesDescriptionManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _feesDescriptionManager.ListAll(pageNo, pageSize, query);
    }

    [HttpGet]
    public async Task<object> GetAllByBillType(Guid billTypeId)
    {
        return await _feesDescriptionManager.ListAll(billTypeId);
    }

    [HttpPost]
    public async Task Create(CreateFeesDescriptionInput input)
    {
        var obj = _mapper.Map<FeesDescription>(input);
         await _feesDescriptionManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateFeesDescriptionInput input)
    {
        var obj = _mapper.Map<FeesDescription>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _feesDescriptionManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _feesDescriptionManager.Delete(id);

    }

}
