using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.PayCalendars;
using RhyoliteOrbit.DomainServices.Payroll.PayCalendars.Dto;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.PayCalendars.Dto;

namespace RhyoliteOrbit.Services.Payroll.PayCalendars;

public class PayCalendarAppService: RhyoliteOrbitAppServiceBase
{
 
    private readonly IPayCalendarManager _payCalendarManager;
    private readonly IMapper _mapper;

    public PayCalendarAppService(IPayCalendarManager payCalendarManager, IMapper mapper)
    {
        _payCalendarManager = payCalendarManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _payCalendarManager.ListAll();
    }

    [HttpGet]
    public async Task<object> GetDefaultAllowanceDays()
    {
        return await _payCalendarManager.GetDefaultAllowanceDays();
    }
    
    [HttpPost]
    public async Task Create(CreatePayCalendarInput input)
    {
        var obj = _mapper.Map<PayCalendar>(input);
        await _payCalendarManager.Create(obj);
    }

    [HttpPost]
    public async Task<object> CreatePayCalendarDetails(PayCalendarDetailDto input)
    {
        return await _payCalendarManager.CreatePayCalendarDetails(input);
    }

    [HttpPost]
    public async Task Update(UpdatePayCalendarInput input)
    {
        var obj = _mapper.Map<PayCalendar>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _payCalendarManager.Update(obj);
    }

    [HttpPost]
    public async Task UpdatePayCalendarDetails(PayCalendarDetailDto input)
    {
        await _payCalendarManager.UpdatePayCalendarDetails(input);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _payCalendarManager.Delete(id);
    }

    [HttpDelete]
    public async Task DeletePayCalendarDetail(PayCalendarDetailDto input)
    {
        await _payCalendarManager.DeletePayCalendarDetails(input);
    }
}