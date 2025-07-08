using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.LoanTypes;

public interface ILoanTypeManager:  Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<LoanType> GetAsync(string name);
    Task<object> Create(LoanType input);
    Task Update(LoanType input);
    Task Delete(Guid id);
}