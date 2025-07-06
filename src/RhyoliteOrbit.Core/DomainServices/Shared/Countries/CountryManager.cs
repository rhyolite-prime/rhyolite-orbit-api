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

    public CountryManager(IRepository<Country, Guid> repositoryCountry)
    {
        _repositoryCountry = repositoryCountry;
    }



    public async Task<object> Create(Country entity)
    {
        var datta = await _repositoryCountry.FirstOrDefaultAsync(x => x.Name == entity.Name);

        if (datta == null)
        {
            await _repositoryCountry.InsertAsync(entity);

            return new
            {
                code = 200,
                message = "successful"
            };
        }

        throw new UserFriendlyException(
            400,
            "Duplicate records are not allowed.",
            "A duplicate Country with the same name already exists."
        );
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
            
        var data = await (await _repositoryCountry.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.CreationTime, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryCountry.DeleteAsync(id);

    }
    
}