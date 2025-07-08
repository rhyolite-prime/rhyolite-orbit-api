using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.SalaryGrades;
using RhyoliteOrbit.DomainServices.Payroll.SalaryGrades.Dto;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.SalaryGrades.Dto;

namespace RhyoliteOrbit.Services.Payroll.SalaryGrades;

public class SalaryGradeAppService: RhyoliteOrbitAppServiceBase
{
    private readonly ISalaryGradeManager _salaryGradeManager;
    private readonly IMapper _mapper;

    public SalaryGradeAppService(ISalaryGradeManager salaryGradeManager, IMapper mapper)
    {
        _salaryGradeManager = salaryGradeManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _salaryGradeManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task<object> Create(CreateSalaryGradeInput input)
    {
        var obj = _mapper.Map<SalaryGrade>(input);
        return await _salaryGradeManager.Create(obj);
    }

    [HttpPost]
    public async Task<object> CreateSalaryNotch(SalaryNotchDto input)
    {
        return await _salaryGradeManager.CreateSalaryNotch(input);
    }

    [HttpPost]
    public async Task Update(UpdateSalaryGradeInput input)
    {
        var obj = _mapper.Map<SalaryGrade>(input);
        await _salaryGradeManager.Update(obj);
    }

    [HttpPost]
    public async Task UpdateSalaryNotch(SalaryNotchDto input)
    {
        await _salaryGradeManager.UpdateSalaryNotch(input);
    }
    
    [HttpDelete]
    public async Task Delete(Guid id)
    {
         await _salaryGradeManager.Delete(id);

    }

    [HttpDelete]
    public async Task DeleteSalaryNotch(SalaryNotchDto input)
    {
        await _salaryGradeManager.DeleteSalaryNotch(input);
    }
}