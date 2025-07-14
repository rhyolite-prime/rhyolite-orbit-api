using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.GeneralLedger;

namespace RhyoliteOrbit.DomainServices.GeneralLedger.CoaHierarchies;

public interface IAccountHeadManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task Create(AccountHead input);
    Task Update(AccountHead input);
    Task Delete(Guid id);
}