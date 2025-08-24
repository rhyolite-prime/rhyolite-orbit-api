using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class BookLending: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid BookId { get; set; }
    public string BookTitle { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; }
    public string StudentClass { get; set; }
    public DateTime LendingDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Remarks { get; set; }
    public int TenantId { get; set; }
}