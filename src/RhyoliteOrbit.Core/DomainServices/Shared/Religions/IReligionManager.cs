using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.Religions;

public interface IReligionManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> Create(Religion input);
    Task<Religion> GetAsync(string name);
    Task Update(Religion input);
    Task Delete(Guid id);
}