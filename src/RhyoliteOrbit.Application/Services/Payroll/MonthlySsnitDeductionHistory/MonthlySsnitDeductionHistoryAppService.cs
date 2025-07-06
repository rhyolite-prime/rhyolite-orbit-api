using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.MonthlySsnitDeductionHistoryRecord;

namespace RhyoliteOrbit.Services.Payroll.MonthlySsnitDeductionHistory;

public class MonthlySsnitDeductionHistoryAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IMonthlySsnitDeductionHistManager _monthlySsnitDeductionHistManager;

    public MonthlySsnitDeductionHistoryAppService(IMonthlySsnitDeductionHistManager monthlySsnitDeductionHistManager)
    {
        _monthlySsnitDeductionHistManager = monthlySsnitDeductionHistManager;
    }

    [HttpGet]
    public async Task<object> GetAll(int month, int year)
    {
        return await _monthlySsnitDeductionHistManager.ListAll(month, year);
    }
}