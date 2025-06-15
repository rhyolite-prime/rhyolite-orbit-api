using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Payroll;

public class EmployeeLoanRepaymentSchedule: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid EmployeeId { get; set; }
    [MaxLength(20)]
    public string EmployeeIdentifier { get; set; }
    [MaxLength(130)]
    public string EmployeeName { get; set; }
    public Guid EmployeeLoanId { get; set; }
    [MaxLength(100)]
    public string EmployeeLoanName { get; set; }
    public DateTime ScheduleDate { get; set; }
    public decimal MonthlyPayment { get; set; }
    public decimal PrincipalPayment { get; set; }
    public decimal InterestPayment { get; set; }
    public decimal PrincipalBalance { get; set; }
    public decimal InterestPlusPrincipalBalance { get; set; }
    public bool IsPaid { get; set; }
    public int TenantId { get; set; }
    public int Period { get; set; }
}