using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Payroll;

public class MonthlyLoanDeductionHist: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid EmployeeId { get; set; }
    [MaxLength(20)]
    public string EmployeeIdentifier { get; set; }
    [MaxLength(130)]
    public string EmployeeName { get; set; }
    [MaxLength(100)]
    public string EmployeeDepartment { get; set; }
    [MaxLength(100)]
    public string EmployeeCategory { get; set; }
    public Guid EmployeeLoanId { get; set; }
    [MaxLength(100)]
    public string EmployeeLoanName { get; set; }
    [MaxLength(150)]
    public string Description { get; set; }
    public Guid LoanTypeId { get; set; }
    [MaxLength(100)]
    public string LoanTypeName { get; set; }
    public int LoanPeriod { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal RepayAmount { get; set; }
    public decimal ClosingBalance { get; set; }
    public decimal OpeningBalance { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public DateTime ScheduleDate { get; set; }
    public decimal MonthlyPayment { get; set; }
    public decimal PrincipalPayment { get; set; }
    public decimal InterestPayment { get; set; }
    public decimal PrincipalBalance { get; set; }
    public decimal InterestPlusPrincipalBalance { get; set; }
    public Guid EmployeeLoanRepaymentScheduleId { get; set; }
        
    public int TenantId { get; set; }
}