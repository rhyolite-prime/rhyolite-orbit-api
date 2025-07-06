using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.Gratuities;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.Gratuities.Dto;

namespace RhyoliteOrbit.Services.Payroll.Gratuities;

public class GratuityAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IGratuityManager _gratuityManager;
    private readonly IMapper _mapper;

    public GratuityAppService(IGratuityManager gratuityManager, IMapper mapper)
    {
        _gratuityManager = gratuityManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize)
    {
        return await _gratuityManager.ListAll(pageNo, pageSize);
    }

    [HttpPost]
    public async Task Create(CreateGratuityInput input)
    {
        var obj = _mapper.Map<Gratuity>(input);
        await _gratuityManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateGratuityInput input)
    {
        var obj = _mapper.Map<Gratuity>(input);
        await _gratuityManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid Id)
    {
        await _gratuityManager.Delete(Id);

    }
}