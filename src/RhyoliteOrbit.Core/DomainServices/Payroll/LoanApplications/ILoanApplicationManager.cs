using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.LoanApplications;

public interface ILoanApplicationManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task Create(LoanApplication input);
    Task Delete(Guid id);
}