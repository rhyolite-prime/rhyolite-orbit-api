using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeSnits;

public interface IEmployeeSnitManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task Create(EmployeeSnit input);
    Task Update(EmployeeSnit input); 
    Task<object> GetAsync(Guid id);
    Task Delete(Guid id);
}