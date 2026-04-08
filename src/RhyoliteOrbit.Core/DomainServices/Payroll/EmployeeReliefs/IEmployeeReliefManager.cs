using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeReliefs;

public interface IEmployeeReliefManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> ListAll(Guid employeeId);
    Task Create(EmployeeRelief input);
    Task Update(EmployeeRelief input);
    Task Delete(Guid id);
    
}