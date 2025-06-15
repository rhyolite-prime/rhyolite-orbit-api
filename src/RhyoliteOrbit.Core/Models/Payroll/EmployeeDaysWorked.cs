using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Payroll;

public class EmployeeDaysWorked: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid EmployeeId { get; set; }
    [MaxLength(20)]
    public string EmployeeIdentifier { get; set; }
    [MaxLength(130)]
    public string EmployeeName { get; set; }
    public int Days { get; set; }
    public decimal Hours { get; set; }
    public decimal Minutes { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public int TenantId { get; set; }
}