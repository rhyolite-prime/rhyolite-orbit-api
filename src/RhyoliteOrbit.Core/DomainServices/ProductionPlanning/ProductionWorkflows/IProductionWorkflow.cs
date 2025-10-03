using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.ProductionPlanning;

namespace RhyoliteOrbit.DomainServices.ProductionPlanning.ProductionWorkflows;

public interface IProductionWorkflow: IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task Create(Models.ProductionPlanning.ProductionWorkflow input);
    Task Update(Models.ProductionPlanning.ProductionWorkflow input);
    Task Delete(Guid id);
}