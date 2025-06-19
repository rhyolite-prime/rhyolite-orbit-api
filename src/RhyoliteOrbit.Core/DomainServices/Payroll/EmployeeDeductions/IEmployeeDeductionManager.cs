using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeDeductions;

public interface IEmployeeDeductionManager
{
    Task<object> ListAll();
    Task<object> ListAll(Guid employeeId);
    Task<object> Create(EmployeeDeduction input);
    Task Update(EmployeeDeduction input);
    Task Delete(Guid id);
}