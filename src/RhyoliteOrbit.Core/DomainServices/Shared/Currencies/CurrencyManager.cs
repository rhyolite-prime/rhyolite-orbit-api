using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.Currencies;

public class CurrencyManager : Abp.Domain.Services.DomainService, ICurrencyManager
{
    private readonly IRepository<Currency, Guid> _repositoryCurrency;

    public CurrencyManager(IRepository<Currency, Guid> repositoryCurrency)
    {
        _repositoryCurrency = repositoryCurrency;
    }

    public async Task Create(Currency entity)
    {
        var datta = await _repositoryCurrency.FirstOrDefaultAsync(x => x.Name == entity.Name );

        if (datta == null)
        {
            await _repositoryCurrency.InsertAsync(entity);

        }
        
        throw new UserFriendlyException(400, "Duplicate records are not allowed.");
    }
    
    public async Task Update(Currency entity)
    {
        await _repositoryCurrency.UpdateAsync(entity);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<Currency>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(a => a.Name.Contains(query));

        }
            
        var totalCount =
                
            await _repositoryCurrency.CountAsync(predicate);
            
        var data = await (await _repositoryCurrency.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.CreationTime, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryCurrency.DeleteAsync(id);

    }
}