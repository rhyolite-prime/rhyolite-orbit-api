using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Payroll;

public class MonthlySalaryAdvanceHist: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
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
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal Amount { get; set; }
    public Guid LoanTypeId { get; set; }
    [MaxLength(100)]
    public string LoanTypeName { get; set; }
    public DateTime LoanDate { get; set; }
    public int TenantId { get; set; }
}