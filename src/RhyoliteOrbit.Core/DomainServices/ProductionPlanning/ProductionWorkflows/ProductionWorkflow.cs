using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.ProductionPlanning;

namespace RhyoliteOrbit.DomainServices.ProductionPlanning.ProductionWorkflows;

public class ProductionWorkflow: DomainService, IProductionWorkflow
{
    private readonly IRepository<Models.ProductionPlanning.ProductionWorkflow, Guid> _repositoryProcessFlow;
        
    public ProductionWorkflow(IRepository<Models.ProductionPlanning.ProductionWorkflow, Guid> repositoryProcessFlow)
    {
        _repositoryProcessFlow = repositoryProcessFlow;
    }

    public async Task Create(Models.ProductionPlanning.ProductionWorkflow entity)
    {
        var datta = await _repositoryProcessFlow.FirstOrDefaultAsync(x => x.Name == entity.Name);

        if (datta == null)
        {
            await _repositoryProcessFlow.InsertAsync(entity);
            return;
        }
        
        throw new UserFriendlyException(400, "Duplicate records are not allowed.");
         
    }

    public async Task Update(Models.ProductionPlanning.ProductionWorkflow entity)
    {
        await _repositoryProcessFlow.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryProcessFlow.GetAsync(id);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<Models.ProductionPlanning.ProductionWorkflow>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Name.ToLower().Contains(query.ToLower()));
        }
            
        var totalCount =
                
            await _repositoryProcessFlow.CountAsync(predicate);
            
        var data = await (await _repositoryProcessFlow.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.CreationTime, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryProcessFlow.DeleteAsync(id);

    }
}