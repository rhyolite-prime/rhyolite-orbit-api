using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeCategories;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.EmployeeCategories.Dto;

namespace RhyoliteOrbit.Services.Payroll.EmployeeCategories;

public class EmployeeCategoryAppService: RhyoliteOrbitAppServiceBase
{
    
    private readonly IEmployeeCategoryManager _employeeCategoryManager;
    private readonly IMapper _mapper;

    public EmployeeCategoryAppService(IEmployeeCategoryManager employeeCategoryManager, IMapper mapper)
    {
        _employeeCategoryManager = employeeCategoryManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _employeeCategoryManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task<object> Create(CreateEmployeeCategoryInput input)
    {
        var obj = _mapper.Map<EmployeeCategory>(input);
        return await _employeeCategoryManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateEmployeeCategoryInput input)
    {
        var obj = _mapper.Map<EmployeeCategory>(input);
        await _employeeCategoryManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _employeeCategoryManager.Delete(id);

    }
    
}