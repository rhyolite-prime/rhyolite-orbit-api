using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Payroll;

public class MonthlyOvertime: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid OvertimeTypeId { get; set; }
    [MaxLength(20)]
    public string OvertimeTypeName { get; set; }
    public int Month { get; set; }
    [MaxLength(100)]
    public string EmployeeDepartment { get; set; }
    [MaxLength(100)]
    public string EmployeeCategory { get; set; }
    public int Year { get; set; }
    public Guid EmployeeId { get; set; }
    [MaxLength(20)]
    public string EmployeeIdentifier { get; set; }
    [MaxLength(130)]
    public string EmployeeName { get; set; }
    public decimal Amount { get; set; }
    public decimal OvertimeHours { get; set; }
    public int TenantId { get; set; }
}