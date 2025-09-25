using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.ProductionPlanning;

namespace RhyoliteOrbit.DomainServices.ProductionPlanning.Schedules;

public class ScheduleManager: DomainService, IScheduleManager
{
    private readonly IRepository<ProductionSchedule, Guid> _repositoryProductionSchedule;
        
    public ScheduleManager(IRepository<ProductionSchedule, Guid> repositoryProductionSchedule)
    {
        _repositoryProductionSchedule = repositoryProductionSchedule;
    }

    public async Task Create(ProductionSchedule entity)
    {
        var datta = await _repositoryProductionSchedule.FirstOrDefaultAsync(x => x.ProductionOrderIdentifier == entity.ProductionOrderIdentifier);

        if (datta == null)
        {
            await _repositoryProductionSchedule.InsertAsync(entity);
            return;
        }
        
        throw new UserFriendlyException(400, "Duplicate records are not allowed.");
         
    }

    public async Task Update(ProductionSchedule entity)
    {
        await _repositoryProductionSchedule.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryProductionSchedule.GetAsync(id);
    }
    
    public async Task UpdatePriorityLevel(Guid id, RhyoliteOrbitConsts.Priority priority)
    {
         var schedule = await _repositoryProductionSchedule.GetAsync(id);
         
         if (schedule != null)
         {
             schedule.Priority = (int)priority;
             await _repositoryProductionSchedule.UpdateAsync(schedule);
         }
    }
    
    public async Task UpdateStatus(Guid id, RhyoliteOrbitConsts.ProductionScheduleStatus scheduleStatus)
    {
        var schedule = await _repositoryProductionSchedule.GetAsync(id);
         
        if (schedule != null)
        {
            schedule.Status = (int)scheduleStatus;
            await _repositoryProductionSchedule.UpdateAsync(schedule);
        }
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<ProductionSchedule>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.ProductionOrderIdentifier.ToLower().Contains(query.ToLower()) || x.ProductName.ToLower().Contains(query.ToLower()) || x.WorkCenterName.Contains(query.ToLower()));
        }
            
        var totalCount =
                
            await _repositoryProductionSchedule.CountAsync(predicate);
            
        var data = await (await _repositoryProductionSchedule.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.ProductName, a.ProductId, a.ProductionOrderId, a.ProductionOrderIdentifier, a.StartDate, a.EndDate, a.WorkCenterName, a.WorkCenterId, a.Priority, a.Notes, a.Status, a.ScheduledQuantity, a.CreationTime, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryProductionSchedule.DeleteAsync(id);

    }
}