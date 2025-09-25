using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.ProductionPlanning;

namespace RhyoliteOrbit.DomainServices.ProductionPlanning.FinalProductTests;

public class FinalProductTestManager: DomainService, IFinalProductTestManager
{
    private readonly IRepository<FinalProductTest, Guid> _repositoryFinalProductTest;
        
    public FinalProductTestManager(IRepository<FinalProductTest, Guid> repositoryFinalProductTest)
    {
        _repositoryFinalProductTest = repositoryFinalProductTest;
    }

    public async Task Create(FinalProductTest entity)
    {
        await _repositoryFinalProductTest.InsertAsync(entity);
    }

    public async Task Update(FinalProductTest entity)
    {
        await _repositoryFinalProductTest.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryFinalProductTest.GetAsync(id);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<FinalProductTest>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.ProductionOrderIdentifier.ToLower().Contains(query.ToLower()) || x.InspectorName.ToLower().Contains(query.ToLower()));
        }
            
        var totalCount =
                
            await _repositoryFinalProductTest.CountAsync(predicate);
            
        var data = await (await _repositoryFinalProductTest.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.ProductionOrderIdentifier, a.CreationTime, a.InspectorId, a.ProductionOrderId, a.Status, a.QcChecklistId, a.QcChecklistName, a.Results, a.TestDate  }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryFinalProductTest.DeleteAsync(id);
    }
}