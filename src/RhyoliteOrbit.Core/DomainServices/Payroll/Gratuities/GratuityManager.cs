using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.Gratuities;

public class GratuityManager: Abp.Domain.Services.IDomainService , IGratuityManager
{
    private readonly IRepository<Gratuity, Guid> _repositoryGratuity;

    public GratuityManager(IRepository<Gratuity, Guid> repositoryGratuity)
    {
        _repositoryGratuity = repositoryGratuity;
    }

    public async Task Create(Gratuity entity)
    {
        await _repositoryGratuity.InsertAsync(entity);

    }

    public async Task Update(Gratuity entity)
    {
        await _repositoryGratuity.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryGratuity.FirstOrDefaultAsync(id);
    }

    public async Task<object> ListAll()
    {
        var data = await _repositoryGratuity.GetAllListAsync();
        return data.OrderBy(a => a.MaxYears).ToList();
    }
    
    public async Task<object> ListAll(int pageNo, int pageSize)
    {
        var predicate = PredicateBuilder.New<Gratuity>();

        predicate.And(x => !x.IsDeleted);
            
         
        var totalCount =
                
            await _repositoryGratuity.CountAsync(predicate);
            
        var data = await (await _repositoryGratuity.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.MinYears, a.MaxYears, a.Factor  }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryGratuity.DeleteAsync(id);

    }
}