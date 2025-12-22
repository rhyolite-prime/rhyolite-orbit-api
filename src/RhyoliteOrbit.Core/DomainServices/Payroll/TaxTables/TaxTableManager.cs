using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.TaxTables;

public class TaxTableManager: Abp.Domain.Services.DomainService, ITaxTableManager
{
    private readonly IRepository<TaxTable, Guid> _repositoryTaxTable;

    public TaxTableManager(IRepository<TaxTable, Guid> repositoryTaxTable)
    {
        _repositoryTaxTable = repositoryTaxTable;
    }

    public async Task Create(TaxTable entity)
    {
        await _repositoryTaxTable.InsertAsync(entity);
    }

    public async Task Update(TaxTable entity)
    {
        await _repositoryTaxTable.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryTaxTable.FirstOrDefaultAsync(id);
    }

    public async Task<object> ListAll(int pageNo, int pageSize)
    {
        var predicate = PredicateBuilder.New<TaxTable>();

        predicate.And(x => !x.IsDeleted);
         
            
        var totalCount =
                
            await _repositoryTaxTable.CountAsync(predicate);
            
        var data = await (await _repositoryTaxTable.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Rate, a.UpperLimitOfAmount, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

        var totalPages = totalCount > 0 ? Convert.ToInt32(Math.Ceiling(totalCount / (decimal)pageSize)) : 0;

        var paginatedResult = new
        {
            pageNo,
            totalCount,
            totalPages,
            data = data.OrderBy(a=> a.Rate).ToList(),
            lowerBound = pageSize * (pageNo - 1) + 1,
            upperBound = totalPages == pageNo ? totalCount : pageNo * pageSize,
        };

        return paginatedResult;
    }
    
    
    public async Task Delete(Guid id)
    {
        await _repositoryTaxTable.DeleteAsync(id);

    }
    
}