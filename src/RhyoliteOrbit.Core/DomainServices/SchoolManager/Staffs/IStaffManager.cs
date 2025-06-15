using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.Staffs;

public interface IStaffManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(bool isTeachingStaff);
    Task<object> GetAsync(Guid id);
    Task<object> Create(Staff input);
    Task Update(Staff input);
    Task Delete(Guid id);
}