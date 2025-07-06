using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.OvertimeTimeSheets;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.OvertimeTimeSheets.Dto;

namespace RhyoliteOrbit.Services.Payroll.OvertimeTimeSheets;

public class OvertimeTimeSheetAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IOvertimeTimeSheetManager _overtimeTimeSheetManager;
    private readonly IMapper _mapper;

    public OvertimeTimeSheetAppService(IOvertimeTimeSheetManager overtimeTimeSheetManager, IMapper mapper)
    {
        _overtimeTimeSheetManager = overtimeTimeSheetManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _overtimeTimeSheetManager.ListAll(pageNo, pageSize, query);
    }

    [HttpGet]
    public async Task<object> GetByMonthAndYear(int month, int year)
    {
        return await _overtimeTimeSheetManager.ListAll(month, year);
    }

    [HttpPost]
    public async Task Create(CreateOvertimeTimeSheetInput input)
    {
        var obj = _mapper.Map<OvertimeTimeSheet>(input);
        await _overtimeTimeSheetManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateOvertimeTimeSheetInput input)
    {
        var obj = _mapper.Map<OvertimeTimeSheet>(input);
        await _overtimeTimeSheetManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _overtimeTimeSheetManager.Delete(id);

    }
}