using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.Branches;

public interface IPosBranchManager: IDomainService
{
    Task<object> GetAll(int pageNo, int pageSize, string query);
    Task Create(PosBranch entity, string dataSupportBusinessAccountId);
    Task Update(PosBranch entity, string dataSupportBusinessAccountId);
    Task SetDefaultBranch(Guid id);
    Task Delete(Guid id, string dataSupportBusinessAccountId);
}