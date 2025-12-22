using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.PayrollPeriods;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.PayrollPeriods.Dto;

namespace RhyoliteOrbit.Services.Payroll.PayrollPeriods;

public class PayrollPeriodAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IPayrollPeriodManager _payrollPeriodManager;
    private readonly IMapper _mapper;

    public PayrollPeriodAppService(IPayrollPeriodManager payrollPeriodManager, IMapper mapper)
    {
        _payrollPeriodManager = payrollPeriodManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetConfiguration()
    {
        return await _payrollPeriodManager.GetData();
    }

    [HttpPost]
    public async Task Create(CreatePayrollPeriodInput input)
    {
        var obj = _mapper.Map<PayrollPeriod>(input);
        await _payrollPeriodManager.Create(obj);
    }
}