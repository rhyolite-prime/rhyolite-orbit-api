using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Payroll;

public class SalaryIncrementHistory: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid EmployeeId { get; set; }
    [MaxLength(20)]
    public string EmployeeIdentifier { get; set; }
    [MaxLength(130)]
    public string EmployeeName { get; set; }
    public decimal CurrentSalary { get; set; }
    public decimal PreviousSalary { get; set; }
    public decimal IncrementAmount { get; set; }
    public int TenantId { get; set; }
}