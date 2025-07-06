using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.DeductionTypes;
using RhyoliteOrbit.DomainServices.Payroll.DeductionTypes.Dto;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.DeductionTypes.Dto;

namespace RhyoliteOrbit.Services.Payroll.DeductionTypes;

public class DeductionTypeAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IDeductionTypeManager _deductionTypeManager;
    private readonly IMapper _mapper;

    public DeductionTypeAppService(IDeductionTypeManager deductionTypeManager, IMapper mapper)
    {
        _deductionTypeManager = deductionTypeManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        return await _deductionTypeManager.ListAll(pageNo, pageSize, query);
    }

    [HttpGet]
    public async Task<object> ListAll(Guid deductionTypeId, Guid employeeId, Guid categoryId)
    {
        return await _deductionTypeManager.ListAll(deductionTypeId, employeeId, categoryId);
    }
    
    [HttpGet]
    public async Task<object> GetByCategory(Guid categoryId)
    {
        return await _deductionTypeManager.ListAll(categoryId);
    }

    [HttpPost]
    public async Task<object> Create(CreateDeductionTypeInput input)
    {
        var obj = _mapper.Map<DeductionType>(input);
        return await _deductionTypeManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateDeductionTypeInput input)
    {
        var obj = _mapper.Map<DeductionType>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _deductionTypeManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
         await _deductionTypeManager.Delete(id);
    }

    [HttpPost]
    public async Task<object> CreateDeductionRate(DeductionRateDto input)
    {
        return await _deductionTypeManager.CreateDeductionRate(input);
    }

    [HttpPost]
    public async Task UpdateDeductionRate(DeductionRateDto input)
    {
        await _deductionTypeManager.UpdateDeductionRate(input);
    }

    [HttpDelete]
    public async Task DeleteDeductionRate(DeductionRateDto input)
    {
        await _deductionTypeManager.DeleteDeductionRate(input);
    }
}