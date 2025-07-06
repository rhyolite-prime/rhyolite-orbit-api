using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.TaxReliefs;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.TaxReliefs.Dto;

namespace RhyoliteOrbit.Services.Payroll.TaxReliefs;

public class TaxReliefAppService:RhyoliteOrbitAppServiceBase
{
    private readonly ITaxReliefManager _taxReliefManager;
    private readonly IMapper _mapper;

    public TaxReliefAppService(ITaxReliefManager taxReliefManager, IMapper mapper)
    {
        _taxReliefManager = taxReliefManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _taxReliefManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task<object> Create(CreateTaxReliefInput input)
    {
        var obj = _mapper.Map<TaxRelief>(input);
        return await _taxReliefManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateTaxReliefInput input)
    {
        var obj = _mapper.Map<TaxRelief>(input);
        await _taxReliefManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _taxReliefManager.Delete(id);

    }
}