using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeReliefs;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.EmployeeReliefs.Dto;

namespace RhyoliteOrbit.Services.Payroll.EmployeeReliefs;

public class EmployeeReliefAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IEmployeeReliefManager _employeeReliefManager;
    private readonly IMapper _mapper;

    public EmployeeReliefAppService(IEmployeeReliefManager employeeReliefManager, IMapper mapper)
    {
        _employeeReliefManager = employeeReliefManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _employeeReliefManager.ListAll(pageNo, pageSize, query);
    }

    [HttpGet]
    public async Task<object> GetReliefsForEmployee(Guid employeeId)
    {
        return await _employeeReliefManager.ListAll(employeeId);
    }

    [HttpPost]
    public async Task<object> Create(CreateEmployeeReliefInput input)
    {
        var obj = _mapper.Map<EmployeeRelief>(input);
        return await _employeeReliefManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateEmployeeReliefInput input)
    {
        var obj = _mapper.Map<EmployeeRelief>(input);
        await _employeeReliefManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _employeeReliefManager.Delete(id);
    }
}