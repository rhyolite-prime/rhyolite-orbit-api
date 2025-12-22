using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.DomainServices.ProductionPlanning.FinalProductTests;
using RhyoliteOrbit.Models.ProductionPlanning;

namespace RhyoliteOrbit.DomainServices.ProductionPlanning.InProcessInspections;

public class InProcessInspectionManager: DomainService, IInProcessInspectionManager
{
    private readonly IRepository<InProcessInspection, Guid> _repositoryInProcessInspection;
        
    public InProcessInspectionManager(IRepository<InProcessInspection, Guid> repositoryInProcessInspection)
    {
        _repositoryInProcessInspection = repositoryInProcessInspection;
    }

    public async Task Create(InProcessInspection entity)
    {
        await _repositoryInProcessInspection.InsertAsync(entity);
    }

    public async Task Update(InProcessInspection entity)
    {
        await _repositoryInProcessInspection.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryInProcessInspection.GetAsync(id);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<InProcessInspection>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.JobTicketIdentifier.ToLower().Contains(query.ToLower()) || x.ProductionOrderIdentifier.ToLower().Contains(query.ToLower()));
        }
            
        var totalCount =
                
            await _repositoryInProcessInspection.CountAsync(predicate);
            
        var data = await (await _repositoryInProcessInspection.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.ProductionOrderId, a.CreationTime, a.ProductionOrderIdentifier , a.JobTicketIdentifier, a.JobTicketId, a.Status, a.InspectionDate, a.InspectorId, a.InspectorName, a.QcChecklistId, a.QcChecklistName, a.Results, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryInProcessInspection.DeleteAsync(id);
    }
}