using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeLoans.Dto;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeLoans;

public interface IEmployeeLoanManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> GetPendingLoans();
    Task<object> GetLoanRepaymentSummary(Guid loanId);
    Task AmortizeLoan(LoanAmortizationDto dto);
    Task<object> GetOutStandingLoans();
    Task<object> GetApprovedLoans();
    Task<object> GetPastLoans();
    Task Approve(List<Guid> ids, string approvalType);
    Task<object> ListAll(Guid employeeId);
    Task Create(EmployeeLoan input);
    Task Update(EmployeeLoan input);
    Task<EmployeeLoan> GetAsync(Guid id);
    Task Delete(Guid id);
}