using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.ProductionPlanning;

namespace RhyoliteOrbit.DomainServices.ProductionPlanning.QcChecklists;

public class QcChecklistManager : DomainService, IQcChecklistManager
{
    private readonly IRepository<QcChecklist, Guid> _repositoryQcChecklist;
        
    public QcChecklistManager(IRepository<QcChecklist, Guid> repositoryQcChecklist)
    {
        _repositoryQcChecklist = repositoryQcChecklist;
    }

    public async Task Create(QcChecklist entity)
    {
        var datta = await _repositoryQcChecklist.FirstOrDefaultAsync(x => x.Name == entity.Name);

        if (datta == null)
        {
            await _repositoryQcChecklist.InsertAsync(entity);
            return;
        }
        
        throw new UserFriendlyException(400, "Duplicate Quality Control checklist are not allowed.");
         
    }

    public async Task Update(QcChecklist entity)
    {
        await _repositoryQcChecklist.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryQcChecklist.GetAsync(id);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<QcChecklist>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Name.ToLower().Contains(query.ToLower()));
        }
            
        var totalCount =
            await _repositoryQcChecklist.CountAsync(predicate);
            
        var data = await (await _repositoryQcChecklist.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.CreationTime, a.Description, a.ProductId, a.ProductName, a.Items  }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryQcChecklist.DeleteAsync(id);
    }
}