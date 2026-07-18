using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.GeneralLedger.Journals;
using RhyoliteOrbit.DomainServices.GeneralLedger.Journals.Dto;
using RhyoliteOrbit.Services.GeneralLedger.Journals.Dto;

namespace RhyoliteOrbit.Services.GeneralLedger.Journals;

public class JournalAppService : RhyoliteOrbitAppServiceBase
{
    private readonly IJournalManager  _journalManager;
    
    public JournalAppService(IJournalManager journalManager)
    {
        _journalManager = journalManager;
    }

    public async Task<object> GetAll(int pageNo, int pageSize, string batchNumber = null, DateTime? startDate = null,
        DateTime? endDate = null, bool? isPosted = null, bool? isMultiCurrency = null)
    {
        return await _journalManager.ListAll(pageNo, pageSize, batchNumber, startDate, endDate, isPosted, isMultiCurrency);
    }
 
    [HttpGet]
    public async Task<object> GetDetails(Guid id)
    {
       return await _journalManager.GetDetails(id);
    }
    
    [HttpPost]
    public async Task Create(JournalDto dto)
    {
        await _journalManager.Create(dto);
    }

    [HttpDelete]
    public async Task BatchDelete(BatchDeleteDto dto)
    {
        await _journalManager.BatchDelete(dto.Ids);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _journalManager.Delete(id);
    }
}