using System.Threading.Tasks;

namespace RhyoliteOrbit.DomainServices.Payroll.MonthlySsnitDeductionHistoryRecord;

public interface IMonthlySsnitDeductionHistManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int month, int year);
}