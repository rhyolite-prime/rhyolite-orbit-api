using System.Threading.Tasks;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.SalaryIncrements;

public interface ISalaryIncrementHistoryManager: Abp.Domain.Services.IDomainService
{
    
    Task<object> ListAll();
    Task Create(SalaryIncrementHistory input);
}