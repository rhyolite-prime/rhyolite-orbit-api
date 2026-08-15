using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.GeneralLedger.CoaDetails;
using RhyoliteOrbit.Models.GeneralLedger;
using RhyoliteOrbit.Services.GeneralLedger.CoaDetails.Dto;

namespace RhyoliteOrbit.Services.GeneralLedger.CoaDetails;

public class CoaDetailAppService: RhyoliteOrbitAppServiceBase
{
    private readonly ICoaDetailManager _coaDetailManager;
    private readonly IMapper _mapper;

    public CoaDetailAppService(ICoaDetailManager coaDetailManager , IMapper mapper)
    {
        _coaDetailManager = coaDetailManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query, RhyoliteOrbitConsts.AccountStatus? status)
    {
        return await _coaDetailManager.ListAll(pageNo, pageSize, query, status);
    }
    
    
    [HttpGet]
    public async Task<object> GetActiveAccounts()
    {
        return await _coaDetailManager.ListActiveAccounts();
    }
    
    [HttpGet]
    public async Task<object> CheckAccountDetails(int accountNumber, Guid accountControlId)
    {
        return await _coaDetailManager.CheckAccountDetails(accountNumber, accountControlId);
    }
    
    [HttpPost]
    public async Task Create(CreateCoaDetailInput input)
    {
        var obj = _mapper.Map<CoaDetail>(input);
        await _coaDetailManager.Create(obj);
    }
    
    
    [HttpPost]
    public async Task Update(UpdateCoaDetailInput input)
    {
        var obj = _mapper.Map<CoaDetail>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _coaDetailManager.Update(obj);
    }

    
    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _coaDetailManager.Delete(id);
    }
}