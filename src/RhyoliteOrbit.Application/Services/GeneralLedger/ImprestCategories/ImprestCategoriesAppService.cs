using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.GeneralLedger.ImprestCategories;
using RhyoliteOrbit.Models.GeneralLedger;
using RhyoliteOrbit.Services.GeneralLedger.ImprestCategories.Dto;

namespace RhyoliteOrbit.Services.GeneralLedger.ImprestCategories;

public class ImprestCategoriesAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IImprestCategoryManager _imprestCategoryManager;
    private readonly IMapper _mapper;

    public ImprestCategoriesAppService(IImprestCategoryManager imprestCategoryManager, IMapper mapper)
    {
        _imprestCategoryManager = imprestCategoryManager;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _imprestCategoryManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task Create(CreateImprestCategoryInput input)
    {
        var obj = _mapper.Map<ImprestCategory>(input);
        await _imprestCategoryManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateImprestCategoryInput input)
    {
        var obj = _mapper.Map<ImprestCategory>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _imprestCategoryManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _imprestCategoryManager.Delete(id);
    }
}