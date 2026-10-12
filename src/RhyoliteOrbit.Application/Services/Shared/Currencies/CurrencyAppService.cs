using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.Shared.Currencies;
using RhyoliteOrbit.Models.Shared;
using RhyoliteOrbit.Services.Shared.Currencies.Dto;

namespace RhyoliteOrbit.Services.Shared.Currencies;


public class CurrencyAppService: RhyoliteOrbitAppServiceBase
{
    private readonly ICurrencyManager  _currencyManager;
    private readonly IValidateAuth _validateAuth;
    private readonly IMapper _mapper;

    public CurrencyAppService(ICurrencyManager currencyManager, IMapper mapper, IValidateAuth validateAuth)
    {
        _currencyManager = currencyManager;
        _mapper = mapper;
        _validateAuth = validateAuth;
    }
    

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _currencyManager.ListAll(pageNo, pageSize, query);
    }
    
    [HttpGet]
    public async Task<object> ListAll()
    {
        var tenantId = await _validateAuth.ValidateTenant();
        
        return await _currencyManager.ListAll(tenantId);
    }
    
    [HttpGet]
    public async Task<object> GetDefaultCurrency()
    {
        var tenantId = await _validateAuth.ValidateTenant();
        
        return await _currencyManager.GetDefaultCurrency(tenantId);
    }

    [HttpPost]
    public async Task Create(CreateCurrencyInput input)
    {
        var obj = _mapper.Map<Currency>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        await _currencyManager.Create(obj, tenantInfo.DataSupportBusinessAccountId);
    }

    [HttpPost]
    public async Task Update(UpdateCurrencyInput input)
    {
        var obj = _mapper.Map<Currency>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _currencyManager.Update(obj, tenantInfo.DataSupportBusinessAccountId);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        
        await _currencyManager.Delete(id, tenantInfo.DataSupportBusinessAccountId);
        
    }
}