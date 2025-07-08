using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.IrsSignatures;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.IrsSignatures.Dto;

namespace RhyoliteOrbit.Services.Payroll.IrsSignatures;

public class IrsSignatureAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IIrsSignatureManager _irsSignatureManager;
    private readonly IMapper _mapper;

    public IrsSignatureAppService(IIrsSignatureManager irsSignatureManager, IMapper mapper)
    {
        _irsSignatureManager = irsSignatureManager;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task Create(CreateIrsSignatureInput input)
    {
        var obj = _mapper.Map<IrsSignature>(input);
        await _irsSignatureManager.Create(obj);
    }

    [HttpGet]
    public async Task<object> GetSignature()
    {
        return await _irsSignatureManager.GetSignature();
    }
}