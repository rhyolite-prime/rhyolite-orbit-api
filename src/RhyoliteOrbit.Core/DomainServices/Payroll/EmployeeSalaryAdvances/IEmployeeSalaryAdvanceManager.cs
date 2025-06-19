using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeSalaryAdvances;

public interface IEmployeeSalaryAdvanceManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task Approve(List<Guid> ids, string approvalType);
    Task<object> ListPendingSalaryAdvance();
    Task<object> GetAsync(Guid id);
    Task<object> ListAll(Guid employeeId);
    Task Create(EmployeeSalaryAdvance input);
    Task Update(EmployeeSalaryAdvance input);
    Task Delete(Guid id);
}