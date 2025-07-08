using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeBenefitInKinds;

public interface IEmployeeBenefitInKindManager:Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> ListAll(Guid employeeId);
    Task<object> Create(EmployeeBenefitInKind entity);
    Task Update(EmployeeBenefitInKind entity);
    Task Delete(Guid id);
}