using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Actors.MessageModels;

namespace RhyoliteOrbit.DomainServices.Payroll.ProcessPay;

public interface IProcessPayManager: Abp.Domain.Services.IDomainService
{
    Task<object> ProcessPayroll(Guid id);
    Task PostPayroll();
    Task<object> GetPayrollResults();
    
    Task<object> ProcessCustomAllowanceOrBonus();
    Task<object> GetSpecialEarningsPayrollResults();
    Task<object> GetSpecialEarningsPayrollResults(Guid employeeId);
    Task<object> GetPayrollResults(Guid employeeId);
    Task DispatchPayslips(PayslipDispatchDto dto);
}