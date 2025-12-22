using System;
using System.Threading.Tasks;
using Abp.Domain.Services;

namespace RhyoliteOrbit.DomainServices.ProductionPlanning.FinishedGoods;

public interface IFinishedGoodManager: IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> GetAsync(Guid id);
    Task Delete(Guid id);
}