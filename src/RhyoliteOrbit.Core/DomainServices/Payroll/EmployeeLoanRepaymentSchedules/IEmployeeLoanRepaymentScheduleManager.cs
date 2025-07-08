using System.Threading.Tasks;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeLoanRepaymentSchedules;

public interface IEmployeeLoanRepaymentScheduleManager : Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
}