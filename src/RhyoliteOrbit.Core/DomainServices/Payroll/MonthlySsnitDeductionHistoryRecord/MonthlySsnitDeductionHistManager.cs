using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.MonthlySsnitDeductionHistoryRecord;

public class MonthlySsnitDeductionHistManager: DomainService, IMonthlySsnitDeductionHistManager
{
    private readonly IRepository<MonthlySsnitDeductionHistory, Guid> _repositoryMonthlySsnitDeductionHist;

    public MonthlySsnitDeductionHistManager(IRepository<MonthlySsnitDeductionHistory, Guid> repositoryMonthlySsnitDeductionHist)
    {
        _repositoryMonthlySsnitDeductionHist = repositoryMonthlySsnitDeductionHist;
    }

         
    public async Task<object> ListAll(int month, int year)
    {
        return await _repositoryMonthlySsnitDeductionHist.GetAllListAsync(x=>x.Month == month && x.Year == year && x.EmployeeId != Guid.Empty);
    }
}