using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.GeneralLedger.CoaHierarchies;
using RhyoliteOrbit.Models.GeneralLedger;
using RhyoliteOrbit.Services.GeneralLedger.AccountHeads.Dto;

namespace RhyoliteOrbit.Services.GeneralLedger.AccountHeads;

public class AccountHeadAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IAccountHeadManager _accountHeadManager;
    private readonly IMapper _mapper;

    public AccountHeadAppService(IAccountHeadManager accountHeadManager, IMapper mapper)
    {
        _accountHeadManager = accountHeadManager;
        _mapper = mapper;
    }
    

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _accountHeadManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task Create(CreateAccountHeadInput input)
    {
        var obj = _mapper.Map<AccountHead>(input);
        await _accountHeadManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateAccountHeadInput input)
    {
        var obj = _mapper.Map<AccountHead>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _accountHeadManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _accountHeadManager.Delete(id);
    }
    
}