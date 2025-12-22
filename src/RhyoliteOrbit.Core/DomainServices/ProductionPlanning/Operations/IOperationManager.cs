using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.ProductionPlanning;

namespace RhyoliteOrbit.DomainServices.ProductionPlanning.JobTickets;

public interface IOperationManager : IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> GetAsync(Guid id);
    Task Create(Operation entity);
    Task Update(Operation entity);
    Task Delete(Guid id);
}