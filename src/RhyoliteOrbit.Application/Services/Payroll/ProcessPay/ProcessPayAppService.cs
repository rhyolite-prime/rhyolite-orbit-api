using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Actors.MessageModels;
using RhyoliteOrbit.DomainServices.Payroll.ProcessPay;
using RhyoliteOrbit.DomainServices.Payroll.Reports.Dto;

namespace RhyoliteOrbit.Services.Payroll.ProcessPay;

public class ProcessPayAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IProcessPayManager _processPayManager;

    public ProcessPayAppService(IProcessPayManager processPayManager)
    {
        _processPayManager = processPayManager;
    }

    [HttpGet]
    public async Task PostPayroll()
    {
        await _processPayManager.PostPayroll();
    }

    [HttpGet]
    public async Task<object> ProcessPayroll(Guid id)
    {
        return await _processPayManager.ProcessPayroll(id);
    }

    //remember to include a DSL for querying and processing payroll based on a custom governance rule/ definition or formula

    [HttpGet]
    public async Task<object> GetSpecialEarningsPayrollResults()
    {
        return await _processPayManager.GetSpecialEarningsPayrollResults();
    }
        
    [HttpGet]
    public async Task<object> GetSpecialEarningsPayrollResults(Guid employeeId)
    {
        return await _processPayManager.GetSpecialEarningsPayrollResults(employeeId);
    }
        
    [HttpGet]
    public async Task<object> GetPayrollResults()
    {
        return await _processPayManager.GetPayrollResults();
    }

    [HttpGet]
    public async Task<object> GetPayrollResults(Guid employeeId)
    {
        return await _processPayManager.GetPayrollResults(employeeId);
    }

    [HttpPost]
    public async Task DispatchPayslips(PayslipEmailRequestDto dto)
    {
        await _processPayManager.DispatchPayslips(new PayslipDispatchDto
        {
            EmployeeIds = dto.EmployeeIds,
            IntervalCount = dto.IntervalCount, 
            BatchSize = dto.BatchSize, 
            EmployeePayslip = new List<EmployeePayslip>(), 
            IntervalType = dto.IntervalType
        });
    }
}