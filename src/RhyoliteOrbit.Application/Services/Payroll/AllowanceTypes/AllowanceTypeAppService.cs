using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.AllowanceTypes;
using RhyoliteOrbit.DomainServices.Payroll.AllowanceTypes.Dto;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.AllowanceTypes.Dto;

namespace RhyoliteOrbit.Services.Payroll.AllowanceTypes;

public class AllowanceTypeAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IAllowanceTypeManager _allowanceTypeManager;
    private readonly IMapper _mapper;

    public AllowanceTypeAppService(IAllowanceTypeManager allowanceTypeManager, IMapper mapper)
    {
        _allowanceTypeManager = allowanceTypeManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _allowanceTypeManager.ListAll(pageNo, pageSize, query);
    }
    
    [HttpGet]
    public async Task<object> GetDetails(Guid id)
    {
        return await _allowanceTypeManager.GetAsync(id);
    }

    [HttpGet]
    public async Task<object> GetAllByCategory(Guid categoryId)
    {
        return await _allowanceTypeManager.ListAll(categoryId);
    }

    [HttpGet]
    public async Task<object> GetAllByAllowanceType(Guid allowanceTypeId, Guid employeeId, Guid categoryId)
    {
        return await _allowanceTypeManager.ListAll(allowanceTypeId, employeeId, categoryId);
    }

    [HttpPost]
    public async Task Create(CreateAllowanceTypeInput input)
    {
        var obj = _mapper.Map<AllowanceType>(input);
        await _allowanceTypeManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateAllowanceTypeInput input)
    {
        
        var obj = _mapper.Map<AllowanceType>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _allowanceTypeManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
         await _allowanceTypeManager.Delete(id);            
    }

    [HttpPost]
    public async Task CreateAllowanceRate(AllowanceRateDto input)
    { 
        await _allowanceTypeManager.CreateAllowanceRate(input);
    }

    [HttpPost]
    public async Task UpdateAllowanceRate(AllowanceRateDto input)
    {
        await _allowanceTypeManager.UpdateAllowanceRate(input);
    }

    [HttpDelete]
    public async Task DeleteAllowanceRate(Guid allowanceTypeId,  Guid id)
    {
        await _allowanceTypeManager.DeleteAllowanceRate(allowanceTypeId, id);
    }
}