using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeDaysWorkedData;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.EmployeeDaysWorkedRecords.Dto;

namespace RhyoliteOrbit.Services.Payroll.EmployeeDaysWorkedRecords;

public class EmployeeDayWorkedAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IEmployeeDaysWorkedManager _employeeDaysWorkedManager;
    private readonly IMapper _mapper;

    public EmployeeDayWorkedAppService(IEmployeeDaysWorkedManager employeeDaysWorkedManager, IMapper mapper)
    {
        _employeeDaysWorkedManager = employeeDaysWorkedManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _employeeDaysWorkedManager.ListAll(pageNo, pageSize, query);
    }

    [HttpGet]
    public async Task<object> GetByMonthYearAndSalaryType(int month, int year, string salaryType)
    {
        return await _employeeDaysWorkedManager.GetByMonthYearAndSalaryType(month, year, salaryType);
    }

    [HttpPost]
    public async Task Create(CreateEmployeeDaysWorkedInput input)
    {
        var obj = _mapper.Map<EmployeeDaysWorked>(input);
        await _employeeDaysWorkedManager.Create(obj);
    }
    
    [HttpPost]
    public async Task Update(UpdateEmployeeDaysWorkedInput input)
    {
        var obj = _mapper.Map<EmployeeDaysWorked>(input);
        await _employeeDaysWorkedManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _employeeDaysWorkedManager.Delete(id);

    }
}