using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.FeesDescriptions;

public interface IFeesDescriptionManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll();
    Task<object> ListAll(Guid billTypeId);
    Task<object> Create(FeesDescription input);
    Task Update(FeesDescription input);
    Task Delete(Guid id);
}