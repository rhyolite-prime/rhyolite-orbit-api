using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Shared.Tags;
using RhyoliteOrbit.Models.Shared;
using RhyoliteOrbit.Services.Shared.Tags.Dto;

namespace RhyoliteOrbit.Services.Shared.Tags;

public class TagAppService : RhyoliteOrbitAppServiceBase
{
    private readonly ITagManager  _tagManager;
    private readonly IMapper _mapper;

    public TagAppService(ITagManager tagManager, IMapper mapper)
    {
        _tagManager = tagManager;
        _mapper = mapper;
    }
    

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, RhyoliteOrbitConsts.ErpModule erpModule, string query)
    {
        return await _tagManager.ListAll(pageNo, pageSize, erpModule, query);
    }

    [HttpPost]
    public async Task Create(CreateTagInput input)
    {
        var obj = _mapper.Map<Tag>(input);
        await _tagManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateTagInput input)
    {
        var obj = _mapper.Map<Tag>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _tagManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _tagManager.Delete(id);
    }
}