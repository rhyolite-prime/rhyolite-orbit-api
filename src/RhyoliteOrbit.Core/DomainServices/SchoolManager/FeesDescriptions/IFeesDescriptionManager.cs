using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.FeesDescriptions;

public interface IFeesDescriptionManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query, Guid billTypeId);
    Task<object> ListAll(Guid billTypeId);
    Task Create(FeesDescription input);
    Task Update(FeesDescription input);
    Task Delete(Guid id);
}