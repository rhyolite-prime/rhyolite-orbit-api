using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Shared.SystemNumbers;
using RhyoliteOrbit.Services.Shared.SystemNumbers.Dto;

namespace RhyoliteOrbit.Services.Shared.SystemNumbers;

public class SystemNumberAppService: RhyoliteOrbitAppServiceBase
{
    private readonly ISystemNumberManager _systemNumberManager;

    public SystemNumberAppService(ISystemNumberManager systemNumberManager)
    {
        _systemNumberManager = systemNumberManager;
    }

    [HttpPost]
    public async Task Create(CreateSystemNumberInput input)
    {
        await _systemNumberManager.Create(input.SystemNumberList);
    }
    
}