using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.BoaAllowances;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.BoaAllowances.Dto;

namespace RhyoliteOrbit.Services.Payroll.BoAllowances;

public class BoAllowanceAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IBoAllowanceManager _boAllowanceManager;
    private readonly IMapper _mapper;

    public BoAllowanceAppService(IBoAllowanceManager boAllowanceManager, IMapper mapper)
    {
        _boAllowanceManager = boAllowanceManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _boAllowanceManager.ListAll(pageNo, pageSize, query);
    }


    [HttpGet]
    public async Task<object> ListAll(int month, int year)
    {
        return await _boAllowanceManager.ListAll(month , year);
    }


    [HttpPost]
    public async Task Create(CreateBoAllowanceInput input)
    {
        var obj = _mapper.Map<BonusAndOnetimeAllowance>(input);
        await _boAllowanceManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateBoAllowanceInput input)
    {
        var obj = _mapper.Map<BonusAndOnetimeAllowance>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _boAllowanceManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _boAllowanceManager.Delete(id);
    }
}