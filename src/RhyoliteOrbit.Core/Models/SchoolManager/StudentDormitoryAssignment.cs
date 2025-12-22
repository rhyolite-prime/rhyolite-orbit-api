using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class StudentDormitoryAssignment: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; }
    public string StudentClass { get; set; }
    public Guid DormitoryId { get; set; }
    public string DormitoryName { get; set; }
    public Guid DormitoryRoomId { get; set; }
    public string DormitoryRoomNumber { get; set; }
    public DateTime AssignmentDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Remarks { get; set; }
    public int TenantId { get; set; }
}