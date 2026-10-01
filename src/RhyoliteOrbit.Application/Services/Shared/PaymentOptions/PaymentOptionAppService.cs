using System;
using System.Threading.Tasks;
using Abp.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Shared.PaymentOptions;
using RhyoliteOrbit.Models.Shared;
using RhyoliteOrbit.Services.Shared.PaymentOptions.Dto;

namespace RhyoliteOrbit.Services.Shared.PaymentOptions;

[AbpAuthorize]
public class PaymentOptionAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IPaymentOptionManager _paymentOptionManager;
    private readonly IMapper _mapper;

    public PaymentOptionAppService(IPaymentOptionManager paymentOptionManager, IMapper mapper)
    {
        _paymentOptionManager = paymentOptionManager;
        _mapper = mapper;
    }
    

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _paymentOptionManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task Create(PaymentOptionDto input)
    {
        var obj = _mapper.Map<PaymentOption>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        await _paymentOptionManager.Create(obj, tenantInfo.DataSupportBusinessAccountId);
    }

    [HttpPut("{id}")]
    public async Task Update([FromBody]PaymentOptionDto dto, [FromRoute]Guid id)
    {
        var obj = _mapper.Map<PaymentOption>(dto);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        obj.Id = id;
        await _paymentOptionManager.Update(obj, tenantInfo.DataSupportBusinessAccountId);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        await _paymentOptionManager.Delete(id, tenantInfo.DataSupportBusinessAccountId);
        
    }
}