using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.SchoolManager.FeesDescriptions;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Services.SchoolManager.FeesDescriptions.Dto;

namespace RhyoliteOrbit.DomainServices.SchoolManager.FeesDescriptions;

public class FeesDescriptionAppService : RhyoliteOrbitAppServiceBase
{
    private readonly IFeesDescriptionManager _feesDescriptionManager;
    private readonly IMapper _mapper;
    public FeesDescriptionAppService(IFeesDescriptionManager feesDescriptionManager, IMapper mapper)
    {
        _feesDescriptionManager = feesDescriptionManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll()
    {
        return await _feesDescriptionManager.ListAll();
    }

    [HttpGet]
    public async Task<object> GetAllByBillType(Guid billTypeId)
    {
        return await _feesDescriptionManager.ListAll(billTypeId);
    }

    [HttpPost]
    public async Task<object> Create(CreateFeesDescriptionInput input)
    {
        var obj = _mapper.Map<FeesDescription>(input);
        return await _feesDescriptionManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateFeesDescriptionInput input)
    {
        var obj = _mapper.Map<FeesDescription>(input);
        await _feesDescriptionManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _feesDescriptionManager.Delete(id);

    }

}
