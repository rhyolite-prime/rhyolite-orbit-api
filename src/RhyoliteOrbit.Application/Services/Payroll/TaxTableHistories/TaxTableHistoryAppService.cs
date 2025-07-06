using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.TaxTableHistories;

namespace RhyoliteOrbit.Services.Payroll.TaxTableHistories;

public class TaxTableHistoryAppService: RhyoliteOrbitAppServiceBase
{
    private readonly ITaxTableHistoryManager _taxTableHistoryManager;

    public TaxTableHistoryAppService(ITaxTableHistoryManager taxTableHistoryManager)
    {
        _taxTableHistoryManager = taxTableHistoryManager;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _taxTableHistoryManager.ListAll(pageNo, pageSize, query);
    }
    
    [HttpGet]
    public async Task Create(string title)
    {
        await _taxTableHistoryManager.Create(title);
    }

    [HttpGet]
    public async Task Restore(Guid id)
    {
        await _taxTableHistoryManager.Restore(id);
    }
    
    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _taxTableHistoryManager.Delete(id);

    }
}