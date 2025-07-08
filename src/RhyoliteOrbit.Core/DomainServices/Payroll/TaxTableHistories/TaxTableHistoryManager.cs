using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.TaxTableHistories;

public class TaxTableHistoryManager:Abp.Domain.Services.DomainService, ITaxTableHistoryManager
{
    private readonly IRepository<TaxTable, Guid> _repositoryTaxTable;
    private readonly IRepository<TaxTableHistory, Guid> _repositoryTaxTableHistory;

    public TaxTableHistoryManager(IRepository<TaxTable, Guid> repositoryTaxTable, IRepository<TaxTableHistory, Guid> repositoryTaxTableHistory)
    {
        _repositoryTaxTable = repositoryTaxTable;
        _repositoryTaxTableHistory = repositoryTaxTableHistory;
    }

    
    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<TaxTableHistory>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Title.Contains(query));
        }
            
        var totalCount =
                
            await _repositoryTaxTableHistory.CountAsync(predicate);
            
        var data = await (await _repositoryTaxTableHistory.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Title, a.TaxTableList,   }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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

    public async Task Create(string title)
    {
        var taxTable = await _repositoryTaxTable.GetAllListAsync();

        var taxTableHistory = new TaxTableHistory
        {
            Title = title,
            TaxTableList = taxTable,
        };

        await _repositoryTaxTableHistory.InsertAsync(taxTableHistory);

    }


    public async Task Restore(Guid id)
    {

        var data = await _repositoryTaxTableHistory.FirstOrDefaultAsync(id);

        if (data != null)
        {
            var currentTaxTableList = await _repositoryTaxTable.GetAllListAsync();

            foreach (var currentTaxTable in currentTaxTableList)
            {
                await _repositoryTaxTable.DeleteAsync(currentTaxTable.Id);
            }


            foreach (var taxTable in data.TaxTableList)
            {
                taxTable.Id = Guid.NewGuid();
                await _repositoryTaxTable.InsertAsync(taxTable);
            }

        }

    }

    public async Task Delete(Guid id)
    {
        await _repositoryTaxTableHistory.DeleteAsync(id);
    }
}