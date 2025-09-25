using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.ProductionPlanning;

namespace RhyoliteOrbit.DomainServices.ProductionPlanning.FinalProductTests;

public interface IFinalProductTestManager : IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> GetAsync(Guid id);
    Task Create(FinalProductTest entity);
    Task Update(FinalProductTest entity);
    Task Delete(Guid id);
}