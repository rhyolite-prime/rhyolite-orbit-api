using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.SchoolManager.PaymentTypes;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Services.SchoolManager.PaymentTypes.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.PaymentTypes;

public class PaymentTypeAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IPaymentTypeManager  _paymentTypeManager;
    private readonly IMapper _mapper;

    public PaymentTypeAppService(IPaymentTypeManager paymentTypeManager, IMapper mapper)
    {
        _paymentTypeManager = paymentTypeManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _paymentTypeManager.GetAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task<object> Create(CreatePaymentTypeInput input)
    {
        var obj = _mapper.Map<PaymentType>(input);
        return await _paymentTypeManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdatePaymentTypeInput input)
    {
        var obj = _mapper.Map<PaymentType>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _paymentTypeManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _paymentTypeManager.Delete(id);
    }
}