using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.SchoolManager.Parents;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Services.SchoolManager.Parents.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.Parents;

public class ParentAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IParentManager  _parentManager;
    private readonly IMapper _mapper;

    public ParentAppService(IParentManager parentManager, IMapper mapper)
    {
        _parentManager = parentManager;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _parentManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task Create(CreateParentInput input)
    {
        var obj = _mapper.Map<Parent>(input);
        await _parentManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateParentInput input)
    {
        var obj = _mapper.Map<Parent>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _parentManager.Update(obj);
    }


    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _parentManager.Delete(id);
    }
}