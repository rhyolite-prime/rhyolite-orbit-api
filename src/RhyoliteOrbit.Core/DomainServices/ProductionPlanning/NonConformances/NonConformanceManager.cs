using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.ProductionPlanning;

namespace RhyoliteOrbit.DomainServices.ProductionPlanning.NonConformances;

public class NonConformanceManager: DomainService, INonConformanceManager
{
    private readonly IRepository<NonConformance, Guid> _repositoryNonConformance;
        
    public NonConformanceManager(IRepository<NonConformance, Guid> repositoryNonConformance)
    {
        _repositoryNonConformance = repositoryNonConformance;
    }

    public async Task Create(NonConformance entity)
    {
        await _repositoryNonConformance.InsertAsync(entity);
         
    }

    public async Task Update(NonConformance entity)
    {
        await _repositoryNonConformance.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryNonConformance.GetAsync(id);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<NonConformance>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.ReportedByName.ToLower().Contains(query.ToLower()) || x.CorrectiveAction.Contains(query.ToLower()) || x.RootCause.Contains(query.ToLower()));
        }
            
        var totalCount =
                
            await _repositoryNonConformance.CountAsync(predicate);
            
        var data = await (await _repositoryNonConformance.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.FinalProductTestingId, a.InProcessInspectionId, a.Status, a.CorrectiveAction, a.ReportedByName, a.Description, a.PreventiveAction, a.ReportedAt, a.ReportedById, a.RootCause, a.Severity, a.CreationTime, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryNonConformance.DeleteAsync(id);
    }
}