using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.ProductionPlanning;

namespace RhyoliteOrbit.DomainServices.ProductionPlanning.InProcessInspections;

public interface IInProcessInspectionManager : IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> GetAsync(Guid id);
    Task Create(InProcessInspection entity);
    Task Update(InProcessInspection entity);
    Task Delete(Guid id);
}