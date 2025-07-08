using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.SalaryIncrements;

namespace RhyoliteOrbit.Services.Payroll.SalaryIncrements;

public class SalaryIncrementAppService: RhyoliteOrbitAppServiceBase
{
    private readonly ISalaryIncrementHistoryManager _salaryIncrementHistoryManager;

    public SalaryIncrementAppService(ISalaryIncrementHistoryManager salaryIncrementHistoryManager)
    {
        _salaryIncrementHistoryManager = salaryIncrementHistoryManager;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _salaryIncrementHistoryManager.ListAll(pageNo, pageSize, query);
    }
}