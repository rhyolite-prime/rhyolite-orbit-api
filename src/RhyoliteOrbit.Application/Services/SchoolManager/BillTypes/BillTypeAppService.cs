using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.SchoolManager.BillTypes;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Services.SchoolManager.BillTypes.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.BillTypes;

public class BillTypeAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IBillTypeManager  _billTypeManager;
    private readonly IMapper _mapper;

    public BillTypeAppService(IBillTypeManager billTypeManager, IMapper mapper)
    {
        _billTypeManager = billTypeManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _billTypeManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task Create(CreateBillTypeInput input)
    {
        var obj = _mapper.Map<BillType>(input);
        await _billTypeManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateBillTypeInput input)
    {
        var obj = _mapper.Map<BillType>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _billTypeManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _billTypeManager.Delete(id);
    }
}