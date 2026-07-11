using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.GeneralLedger.BankAccounts;
using RhyoliteOrbit.Models.GeneralLedger;
using RhyoliteOrbit.Models.Shared;
using RhyoliteOrbit.Services.GeneralLedger.BankAccounts.Dto;
using RhyoliteOrbit.Services.Shared.UnitOfMeasurements.Dto;

namespace RhyoliteOrbit.Services.GeneralLedger.BankAccounts;

public class BankAccountAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IBankAccountManager _bankAccountManager;
    private readonly IMapper _mapper;

    public BankAccountAppService(IBankAccountManager rawMaterialManager, IMapper mapper)
    {
        _bankAccountManager = rawMaterialManager;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _bankAccountManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task Create(CreateBankAccountInput input)
    {
        var obj = _mapper.Map<BankAccount>(input);
        await _bankAccountManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateBankAccountInput input)
    {
        var obj = _mapper.Map<BankAccount>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _bankAccountManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _bankAccountManager.Delete(id);
    }
}