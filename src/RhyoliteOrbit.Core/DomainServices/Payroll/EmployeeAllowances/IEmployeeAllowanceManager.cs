using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeAllowances;

public interface IEmployeeAllowanceManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> ListAll(Guid employeeId);
    Task<object> Create(EmployeeAllowance input);
    Task Update(EmployeeAllowance input);
    Task Delete(Guid id);
}