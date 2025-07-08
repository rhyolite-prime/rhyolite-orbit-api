using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.TaxTables;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.TaxTables.Dto;

namespace RhyoliteOrbit.Services.Payroll.TaxTables;

public class TaxTableAppService: RhyoliteOrbitAppServiceBase
{
    private readonly ITaxTableManager _taxTableManager;
    private readonly IMapper _mapper;

    public TaxTableAppService(ITaxTableManager taxTableManager, IMapper mapper)
    {
        _taxTableManager = taxTableManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize)
    {
        return await _taxTableManager.ListAll(pageNo, pageSize);
    }

    [HttpPost]
    public async Task Create(CreateTaxTableInput input)
    {
        var obj = _mapper.Map<TaxTable>(input);
        await _taxTableManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateTaxTableInput input)
    {
        var obj = _mapper.Map<TaxTable>(input);
        await _taxTableManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _taxTableManager.Delete(id);
    }
}