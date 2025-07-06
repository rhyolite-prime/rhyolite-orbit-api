using System.Threading.Tasks;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.SalaryIncrements;

public interface ISalaryIncrementHistoryManager: Abp.Domain.Services.IDomainService
{

    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task Create(SalaryIncrementHistory input);
}