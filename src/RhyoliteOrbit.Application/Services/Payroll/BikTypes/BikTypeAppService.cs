using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.BikTypes;
using RhyoliteOrbit.DomainServices.Payroll.BikTypes.Dto;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.BikTypes.Dto;

namespace RhyoliteOrbit.Services.Payroll.BikTypes;

public class BikTypeAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IBikTypeManager _bikTypeManager;
    private readonly IMapper _mapper;

    public BikTypeAppService(IBikTypeManager bikTypeManager, IMapper mapper)
    {
        _bikTypeManager = bikTypeManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _bikTypeManager.ListAll(pageNo, pageSize, query);
    }

    [HttpGet]
    public async Task<object> GetDetails(Guid id)
    {
        return await _bikTypeManager.GetAsync(id);
    }

    [HttpGet]
    public async Task<object> GetAllByBenefitsInKindType(Guid bikTypeId, Guid employeeId, Guid categoryId)
    {
        return await _bikTypeManager.ListAll(bikTypeId, employeeId, categoryId);
    }
    
    [HttpPost]
    public async Task<object> Create(CreateBikTypeInput input)
    {
        var obj = _mapper.Map<BikType>(input);
        return await _bikTypeManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateBikTypeInput input)
    {
        var obj = _mapper.Map<BikType>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _bikTypeManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
         await _bikTypeManager.Delete(id);
    }

    [HttpPost]
    public async Task CreateBikRate(BikRateDto input)
    {
         await _bikTypeManager.CreateBikRate(input);
    }

    [HttpPost]
    public async Task UpdateBikRate(BikRateDto input)
    {
        await _bikTypeManager.UpdateBikRate(input);
    }

    [HttpDelete]
    public async Task DeleteBikRate(Guid bikTypeId, Guid id)
    {
        await _bikTypeManager.DeleteBikRate(bikTypeId, id);
    }
}