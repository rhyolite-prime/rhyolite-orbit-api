using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.Tags;

public interface ITagManager: IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, RhyoliteOrbitConsts.ErpModule erpModule, string query);
    Task Create(Tag input);
    Task Update(Tag input);
    Task Delete(Guid id);
}