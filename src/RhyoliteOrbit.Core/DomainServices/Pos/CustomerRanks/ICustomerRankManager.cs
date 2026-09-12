using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.CustomerRanks;

public interface ICustomerRankManager: IDomainService
{
    Task<object> GetAll(int pageNo, int pageSize, string query);
    Task Create(CustomerRank input);
    Task Update(CustomerRank input);
    Task Delete(Guid id);
}