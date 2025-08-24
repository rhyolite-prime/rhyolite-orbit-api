using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class StudentClubMembership: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; }
    public string StudentClass { get; set; }
    public Guid ClubId { get; set; }
    public Guid ClubName { get; set; }
    public DateTime JoinDate { get; set; }
    public string Role { get; set; }
    public int TenantId { get; set; }
}