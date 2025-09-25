using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.ProductionPlanning;

namespace RhyoliteOrbit.DomainServices.ProductionPlanning.Schedules;

public interface IScheduleManager : IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task Create(ProductionSchedule entity);
    Task UpdatePriorityLevel(Guid id, RhyoliteOrbitConsts.Priority priority);
    
    Task UpdateStatus(Guid id, RhyoliteOrbitConsts.ProductionScheduleStatus scheduleStatus);
    Task<object> GetAsync(Guid id);
    Task Update(ProductionSchedule entity);
    Task Delete(Guid id);
}