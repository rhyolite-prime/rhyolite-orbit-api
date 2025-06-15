using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Payroll;

public class SalaryAdvanceApplication: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid EmployeeId { get; set; }
    [MaxLength(20)]
    public string EmployeeIdentifier { get; set; }
    [MaxLength(130)]
    public string EmployeeName { get; set; }
    public DateTime ApplyDate { get; set; }
    public DateTime ApprovedDate { get; set; }
    public decimal LoanAmount { get; set; }
    public Guid LoanTypeId { get; set; }
    [MaxLength(100)]
    public Guid LoanTypeName { get; set; }
    [MaxLength(150)]
    public string Reasons { get; set; }
    [MaxLength(20)]
    public string LoanStatus { get; set; }
    //for authentication purposes
    public long UserId { get; set; }
    public int TenantId { get; set; }
}