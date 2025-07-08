using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeOnetimeDeductions;

public interface IEmployeeOnetimeDeductionManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> ListAll(Guid employeeId);
    Task<object> ListAll(int month, int year);
    Task Create(EmployeeOnetimeDeduction input);
    Task Update(EmployeeOnetimeDeduction input);
    Task Delete(Guid id);
}