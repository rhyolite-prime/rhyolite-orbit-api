using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeRanks;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.EmployeeRanks.Dto;

namespace RhyoliteOrbit.Services.Payroll.EmployeeRanks;

public class EmployeeRankAppService:RhyoliteOrbitAppServiceBase
{
    private readonly IEmployeeRankManager _employeeRankManager;
    private readonly IMapper _mapper;

    public EmployeeRankAppService(IEmployeeRankManager employeeRankManager, IMapper mapper)
    {
        _employeeRankManager = employeeRankManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _employeeRankManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task Create(CreateEmployeeRankInput input)
    {
        var obj = _mapper.Map<EmployeeRank>(input);
        await _employeeRankManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateEmployeeRankInput input)
    {
        var obj = _mapper.Map<EmployeeRank>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _employeeRankManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _employeeRankManager.Delete(id);
    }
}