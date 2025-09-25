using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.ProductionPlanning;

namespace RhyoliteOrbit.DomainServices.ProductionPlanning.ProcessFlows;

public interface IProcessFlowManager: IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task Create(ProcessFlow input);
    Task Update(ProcessFlow input);
    Task Delete(Guid id);
}