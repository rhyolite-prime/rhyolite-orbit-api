using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.ProductionPlanning;

namespace RhyoliteOrbit.DomainServices.ProductionPlanning.FinishedGoods;

public class FinishedGoodManager: DomainService, IFinishedGoodManager
{
    private readonly IRepository<FinishedGood, Guid> _repositoryFinishedGood;
        
    public FinishedGoodManager(IRepository<FinishedGood, Guid> repositoryFinishedGood)
    {
        _repositoryFinishedGood = repositoryFinishedGood;
    }
    
    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryFinishedGood.GetAsync(id);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<FinishedGood>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.ProductionOrderIdentifier.ToLower().Contains(query.ToLower()));
        }
            
        var totalCount =
                
            await _repositoryFinishedGood.CountAsync(predicate);
            
        var data = await (await _repositoryFinishedGood.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.IsQaApproved, a.ProductionOrderIdentifier, a.ProductionOrderId, a.ProductId, a.ProductName, a.QuantityProduced, a.CreationTime, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

        var totalPages = totalCount > 0 ? Convert.ToInt32(Math.Ceiling(totalCount / (decimal)pageSize)) : 0;

        var paginatedResult = new
        {
            pageNo,
            pageSize,
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
        await _repositoryFinishedGood.DeleteAsync(id);
    }
}