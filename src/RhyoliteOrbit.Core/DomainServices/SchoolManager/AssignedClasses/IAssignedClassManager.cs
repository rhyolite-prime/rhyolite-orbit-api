using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.AssignedClasses;

public interface IAssignedClassManager: IDomainService
{
    Task<object> Create(AssignedClass input);
    Task Delete(Guid id);
    Task<object> ListAll(int pageNo, int pageSize, string query);
}