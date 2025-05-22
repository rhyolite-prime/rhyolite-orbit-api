using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.Departments;

public interface IDepartmentManager : Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int id, int creationTime, string name);
    
    Task Create(Department input);
    
    Task Update(Department input);
    
    Task Delete(Guid id);
}