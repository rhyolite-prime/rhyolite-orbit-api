using System.Threading.Tasks;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.PayrollPeriods;

public interface IPayrollPeriodManager: Abp.Domain.Services.IDomainService
{
    Task<object> GetData();
    
    Task Create(PayrollPeriod entity);
}