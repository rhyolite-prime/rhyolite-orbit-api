using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Shared.SystemNumbers;
using RhyoliteOrbit.Models.Shared;
using RhyoliteOrbit.Services.Shared.SystemNumbers.Dto;

namespace RhyoliteOrbit.Services.Shared.SystemNumbers;

public class SystemNumberAppService: RhyoliteOrbitAppServiceBase
{
    private readonly ISystemNumberManager _systemNumberManager;
    private readonly IMapper _mapper;
    public SystemNumberAppService(ISystemNumberManager systemNumberManager, IMapper mapper)
    {
        _systemNumberManager = systemNumberManager;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task Create(CreateSystemNumberInput input)
    {
        await _systemNumberManager.Create(input.SystemNumberList);
    }
    
    [HttpPost]
    public async Task Update(UpdateSystemNumberInput input)
    {
        
        var obj = _mapper.Map<SystemNumber>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _systemNumberManager.Update(obj);
    }
    
    [HttpGet]
    public async Task<object> GetAll([FromQuery]string key)
    {
        return await _systemNumberManager.ListAll(key);
    }
    
}