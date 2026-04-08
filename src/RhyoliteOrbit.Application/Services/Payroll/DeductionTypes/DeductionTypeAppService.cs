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
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _deductionTypeManager.ListAll(pageNo, pageSize, query);
    }
    
    [HttpGet]
    public async Task<object> GetDetails(Guid id)
    {
        return await _deductionTypeManager.GetAsync(id);
    }

    [HttpGet]
    public async Task<object> GetAllByDeductionType(Guid deductionTypeId, Guid employeeId, Guid categoryId)
    {
        return await _deductionTypeManager.ListAll(deductionTypeId, employeeId, categoryId);
    }
    
    [HttpGet]
    public async Task<object> GetByCategory(Guid categoryId)
    {
        return await _deductionTypeManager.ListAll(categoryId);
    }

    [HttpPost]
    public async Task Create(CreateDeductionTypeInput input)
    {
        var obj = _mapper.Map<DeductionType>(input);
        await _deductionTypeManager.Create(obj);
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
    public async Task CreateDeductionRate(DeductionRateDto input)
    {
         await _deductionTypeManager.CreateDeductionRate(input);
    }

    [HttpPost]
    public async Task UpdateDeductionRate(DeductionRateDto input)
    {
        await _deductionTypeManager.UpdateDeductionRate(input);
    }

    [HttpDelete]
    public async Task DeleteDeductionRate(Guid deductionTypeId, Guid id)
    {
        await _deductionTypeManager.DeleteDeductionRate(deductionTypeId, id);
    }
}