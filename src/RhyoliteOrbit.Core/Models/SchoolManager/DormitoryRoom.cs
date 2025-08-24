using System;

namespace RhyoliteOrbit.Models.SchoolManager;
public class DormitoryRoom: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid DormitoryId { get; set; }
    public string RoomNumber { get; set; }
    public int Capacity { get; set; }
    public string Description { get; set; }
    public int TenantId { get; set; }
}

