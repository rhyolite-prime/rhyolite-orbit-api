using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.InitializePayMonths;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.InitializePayMonths.Dto;

namespace RhyoliteOrbit.Services.Payroll.InitializePayMonths;

public class InitializePayMonthAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IInitializePayMonthManager _initializePayMonthManager;
    private readonly IMapper _mapper;

    public InitializePayMonthAppService(IInitializePayMonthManager initializePayMonthManager, IMapper mapper)
    {
        _initializePayMonthManager = initializePayMonthManager;
        _mapper = mapper;
    }


    [HttpGet]
    public async Task<object> GetConfiguration()
    {
        return await _initializePayMonthManager.GetData();
    }

    [HttpPost]
    public async Task Create(CreateInitializePayMonthInput input)
    {
        var obj = _mapper.Map<InitializePayMonth>(input);
        await _initializePayMonthManager.Create(obj);
    }
}