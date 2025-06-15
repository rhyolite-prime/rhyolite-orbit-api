using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Payroll;

public class EmployeeRelief: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid EmployeeId { get; set; }
    [MaxLength(20)]
    public string EmployeeIdentifier { get; set; }
    [MaxLength(130)]
    public string EmployeeName { get; set; }
    public Guid ReliefTypeId { get; set; }
    [MaxLength(100)]
    public string ReliefTypeName { get; set; }
    public decimal Amount { get; set; }
    public int TenantId { get; set; }
    
}