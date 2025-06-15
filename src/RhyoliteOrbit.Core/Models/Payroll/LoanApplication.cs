using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Payroll;

public class LoanApplication: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid EmployeeId { get; set; }
    [MaxLength(20)]
    public string EmployeeIdentifier { get; set; }
    [MaxLength(130)]
    public string EmployeeName { get; set; }
    public long UserId { get; set; }
    public DateTime ApplyDate { get; set; }
    public DateTime ApprovedDate { get; set; }
    public decimal LoanAmount { get; set; }
    public Guid LoanTypeId { get; set; }
    [MaxLength(100)]
    public string LoanTypeName { get; set; }
    public string LoanNotes { get; set; }
    [MaxLength(50)]
    public string LoanStatus { get; set; }
    public int LoanDuration { get; set; }
    public DateTime DeductionStarts { get; set; }
    public int TenantId { get; set; }
}