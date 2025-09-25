using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.Staffs;

public interface IStaffManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(bool isTeachingStaff);
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> GetAsync(Guid id);
    Task Create(Staff input);
    Task RegisterViaSelfService(Staff entity);
    Task Update(Staff input);
    Task Delete(Guid id);
}