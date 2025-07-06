using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeLoanRepaymentSchedules;

namespace RhyoliteOrbit.Services.Payroll.EmployeeLoanRepaymentSchedules;

public class EmployeeLoanRepaymentScheduleAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IEmployeeLoanRepaymentScheduleManager _employeeLoanRepaymentScheduleManager;

    public EmployeeLoanRepaymentScheduleAppService(IEmployeeLoanRepaymentScheduleManager employeeLoanRepaymentScheduleManager)
    {
        _employeeLoanRepaymentScheduleManager = employeeLoanRepaymentScheduleManager;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _employeeLoanRepaymentScheduleManager.ListAll(pageNo, pageSize, query);
    }
}