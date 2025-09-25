using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.ProductionPlanning;

namespace RhyoliteOrbit.DomainServices.ProductionPlanning.WorkCenters;

public class WorkCenterManager: DomainService, IWorkCenterManager
{
    
    private readonly IRepository<WorkCenter, Guid> _repositoryWorkCenter;
        
    public WorkCenterManager(IRepository<WorkCenter, Guid> repositoryWorkCenter)
    {
        _repositoryWorkCenter = repositoryWorkCenter;
    }

    public async Task Create(WorkCenter entity)
    {
        var datta = await _repositoryWorkCenter.FirstOrDefaultAsync(x => x.Name == entity.Name);

        if (datta == null)
        {
            await _repositoryWorkCenter.InsertAsync(entity);
            return;
        }
        
        throw new UserFriendlyException(400, "Duplicate records are not allowed.");
         
    }

    public async Task Update(WorkCenter entity)
    {
        await _repositoryWorkCenter.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryWorkCenter.GetAsync(id);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<WorkCenter>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Name.ToLower().Contains(query.ToLower()));
        }
            
        var totalCount =
                
            await _repositoryWorkCenter.CountAsync(predicate);
            
        var data = await (await _repositoryWorkCenter.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.CreationTime, a.CapacityPerHour, a.SupervisorEmail, a.SupervisorId, a.SupervisorName, a.SupervisorPhoneNo, a.OperatingHours  }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryWorkCenter.DeleteAsync(id);
    }
    
    
}