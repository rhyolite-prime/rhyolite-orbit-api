using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.Countries;

public class CountryManager: Abp.Domain.Services.DomainService, ICountryManager
{
    
    private readonly IRepository<Country, Guid> _repositoryCountry;
    private readonly IRepository<Customer, Guid> _repositoryCustomer;
    private readonly IRepository<Region, Guid> _repositoryRegion;

    public CountryManager(IRepository<Country, Guid> repositoryCountry, IRepository<Region, Guid> repositoryRegion, IRepository<Customer, Guid> repositoryCustomer)
    {
        _repositoryCountry = repositoryCountry;
        _repositoryRegion = repositoryRegion;
        _repositoryCustomer = repositoryCustomer;
    }



    public async Task Create(Country entity)
    {
        await _repositoryCountry.InsertAsync(entity);
    }

    public async Task Update(Country entity)
    {
        await _repositoryCountry.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryCountry.GetAsync(id);
    }

    public async Task<Country> GetNationality(string nationality)
    {
        return await _repositoryCountry.FirstOrDefaultAsync(x=> x.Nationality == nationality);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<Country>();
        
        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(a => a.Name.Contains(query));
        }
        
        var totalCount =
            await _repositoryCountry.CountAsync(predicate);
            
        var data = await (await _repositoryCountry.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.IsoCode, a.NumericIsoCode }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

        var totalPages = totalCount > 0 ? Convert.ToInt32(Math.Ceiling(totalCount / (decimal)pageSize)) : 0;

        var paginatedResult = new
        {
            pageNo,
            totalCount,
            totalPages,
            data,
            lowerBound = pageSize * (pageNo - 1) + 1,
            upperBound = totalPages == pageNo ? totalCount : pageNo * pageSize,
        };

        return paginatedResult;
    }
    
    public async Task Delete(Guid id)
    {
        var country = await _repositoryCountry.FirstOrDefaultAsync(id);
        await _repositoryCountry.HardDeleteAsync(country);
    }
    
    //regions
    public async Task<object> ListRegions(string defaultRegion)
    {
        var countryInfo = await _repositoryCountry.FirstOrDefaultAsync(x=> x.IsoCode == defaultRegion);

        var customerRegions = await _repositoryRegion.GetAllListAsync(a=> a.CountryId == countryInfo.Id);
        
        var data = customerRegions.Select(a=> new { a.Id, a.Name, a.Code, a.CountryId, a.Cities }).ToList();

        return data;
    }

    public async Task CreateRegion(Region input)
    {
        await _repositoryRegion.InsertAsync(input);
    }

    public async Task UpdateRegion(Region input)
    {
        await _repositoryRegion.UpdateAsync(input);
    }

    public async Task DeleteRegion(Guid id)
    {
        var region = await _repositoryRegion.FirstOrDefaultAsync(id);
        await _repositoryRegion.HardDeleteAsync(region);
    }
}