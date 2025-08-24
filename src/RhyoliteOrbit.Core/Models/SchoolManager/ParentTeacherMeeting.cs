using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.SchoolManager;

public class ParentTeacherMeeting: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public DateTime MeetingDate { get; set; }
    public string Venue { get; set; }
    public string Agenda { get; set; }
    public string Minutes { get; set; }
    [Column(TypeName = "jsonb")] public List<PtaAttendance> Attendance { get; set; }
    public int TenantId { get; set; }
}