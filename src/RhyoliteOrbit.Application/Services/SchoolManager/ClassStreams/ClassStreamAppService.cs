using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.SchoolManager.ClassStreams;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Services.SchoolManager.ClassStreams.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.ClassStreams;

public class ClassStreamAppService:RhyoliteOrbitAppServiceBase
{
    private readonly IClassStreamManager  _classStreamManager;
    private readonly IMapper _mapper;

    public ClassStreamAppService(IClassStreamManager classStreamManager, IMapper mapper)
    {
        _classStreamManager = classStreamManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _classStreamManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task Create(CreateClassStreamInput input)
    {
        var obj = _mapper.Map<ClassStream>(input);
        await _classStreamManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateClassStreamInput input)
    {
        var obj = _mapper.Map<ClassStream>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _classStreamManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _classStreamManager.Delete(id);

    }
}