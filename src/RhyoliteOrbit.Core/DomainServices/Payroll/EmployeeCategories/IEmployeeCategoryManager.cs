using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeCategories;

public interface IEmployeeCategoryManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> Create(EmployeeCategory input);
    Task<EmployeeCategory> GetAsync(string name);
    Task Update(EmployeeCategory input);
    Task Delete(Guid id);
}