using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Payroll;

public class OvertimeTimeSheet: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid EmployeeId { get; set; }
    [MaxLength(20)]
    public string EmployeeIdentifier { get; set; }
    [MaxLength(100)]
    public string EmployeeName { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    [MaxLength(150)]
    public string JobDescription { get; set; }
    public Guid OvertimeTypeId { get; set; }
    [MaxLength(100)]
    public string OvertimeTypeName { get; set; }
    public decimal OvertimeHours { get; set; }
    public decimal OvertimeMinutes { get; set; }
    public bool Taxable { get; set; }
    public int TenantId { get; set; }
}