using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeOnetimeDeductions;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.EmployeeOnetimeDeductions.Dto;

namespace RhyoliteOrbit.Services.Payroll.EmployeeOnetimeDeductions;

public class EmployeeOnetimeDeductionAppService: RhyoliteOrbitAppServiceBase
{
 
    private readonly IEmployeeOnetimeDeductionManager _employeeOnetimeDeductionManager;
    private readonly IMapper _mapper;

    public EmployeeOnetimeDeductionAppService(IEmployeeOnetimeDeductionManager employeeOnetimeDeductionManager, IMapper mapper)
    {
        _employeeOnetimeDeductionManager = employeeOnetimeDeductionManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _employeeOnetimeDeductionManager.ListAll(pageNo, pageSize, query);
    }
    
    [HttpGet]
    public async Task<object> GetOnetimeDeductionsForEmployee(Guid employeeId)
    {
        return await _employeeOnetimeDeductionManager.ListAll(employeeId);
    }

    [HttpGet]
    public async Task<object> GetOnetimeDeductionsForMonthAndYear(int month, int year)
    {
        return await _employeeOnetimeDeductionManager.ListAll(month,year);

    }

    [HttpPost]
    public async Task Create(CreateEmployeeOnetimeDeductionInput input)
    {
        var obj = _mapper.Map<EmployeeOnetimeDeduction>(input);
        await _employeeOnetimeDeductionManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateEmployeeOnetimeDeductionInput input)
    {
        var obj = _mapper.Map<EmployeeOnetimeDeduction>(input);
        await _employeeOnetimeDeductionManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _employeeOnetimeDeductionManager.Delete(id);

    }
}