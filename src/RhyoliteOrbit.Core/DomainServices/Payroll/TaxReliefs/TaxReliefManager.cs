using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.TaxReliefs;

public class TaxReliefManager: Abp.Domain.Services.DomainService, ITaxReliefManager
{
    private readonly IRepository<TaxRelief, Guid> _repositoryTaxRelief;

    public TaxReliefManager(IRepository<TaxRelief, Guid> repositoryTaxRelief)
    {
        _repositoryTaxRelief = repositoryTaxRelief;
    }

    public async Task<object> Create(TaxRelief entity)
    {
        var datta = await _repositoryTaxRelief.FirstOrDefaultAsync(x => x.Name.Trim().ToLower() == entity.Name.Trim().ToLower());

        if (datta == null)
        {
            await _repositoryTaxRelief.InsertAsync(entity);

            return new
            {
                code = 200,
                message = "successful"
            };
        }

        throw new UserFriendlyException(
            400,
            "Duplicate records are not allowed.",
            "A Tax Relief with the same name already exists."
        );
        
         
    }

    public async Task Update(TaxRelief entity)
    {
        await _repositoryTaxRelief.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryTaxRelief.FirstOrDefaultAsync(id);
    }

    
    
    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<TaxRelief>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Name.Contains(query));
        }
            
        var totalCount =
                
            await _repositoryTaxRelief.CountAsync(predicate);
            
        var data = await (await _repositoryTaxRelief.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.Amount,   }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryTaxRelief.DeleteAsync(id);

    }
}