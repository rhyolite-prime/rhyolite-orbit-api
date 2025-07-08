using System;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeLoans.Dto;

public class LoanAmortizationDto
{
    public decimal AmountPaid { get; set; }
    public Guid BankGlAccountId { get; set; }
    public Guid EmployeeLoanId { get; set; }
}