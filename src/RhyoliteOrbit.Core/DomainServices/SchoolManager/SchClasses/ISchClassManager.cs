using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.SchClasses;

public interface ISchClassManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(Guid levelId);
    Task<object> ListAll();
    Task<object> Create(SchClass input);
    Task<object> GetAsync(Guid id);
    Task Update(SchClass input);
    Task Delete(Guid id);
    
}