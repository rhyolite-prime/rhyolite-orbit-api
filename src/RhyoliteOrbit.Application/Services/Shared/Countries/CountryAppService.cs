using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Shared.Countries;
using RhyoliteOrbit.Models.Shared;
using RhyoliteOrbit.Services.Shared.Countries.Dto;

namespace RhyoliteOrbit.Services.Shared.Countries;

public class CountryAppService: RhyoliteOrbitAppServiceBase
{
    private readonly ICountryManager  _countryManager;
    private readonly IMapper _mapper;

    public CountryAppService(ICountryManager currencyManager, IMapper mapper)
    {
        _countryManager = currencyManager;
        _mapper = mapper;
    }
    

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _countryManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task Create(CountryDto input)
    {
        var obj = _mapper.Map<Country>(input);
        await _countryManager.Create(obj);
    }

    [HttpPut("{id}")]
    public async Task Update([FromBody]CountryDto input, [FromRoute]Guid id)
    {
        var obj = _mapper.Map<Country>(input);
        await _countryManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _countryManager.Delete(id);
    }
    
    // regions
    [HttpPost]
    public async Task CreateRegion(RegionDto input)
    {
        var obj = _mapper.Map<Region>(input);
        await _countryManager.CreateRegion(obj);
    }
    
    //cities
    
}