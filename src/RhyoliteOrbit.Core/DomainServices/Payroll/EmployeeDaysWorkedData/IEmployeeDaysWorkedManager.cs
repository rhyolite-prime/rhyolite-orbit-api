using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeDaysWorkedData;

public interface IEmployeeDaysWorkedManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> GetByMonthYearAndSalaryType(int month, int year, string salaryType);
    Task Create(EmployeeDaysWorked input);
    Task Update(EmployeeDaysWorked input);
    Task Delete(Guid id);
}