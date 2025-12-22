using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.SchoolManager;

public class FieldTrip: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime TripDate { get; set; }
    public string Venue { get; set; }
    public string Organizer { get; set; }
    public int NumberOfParticipants { get; set; }
    public int Status { get; set; }
    public Guid LeadTeacherId { get; set; } // Lead teacher
    public string LeadTeacherName { get; set; }
    public string LeadTeacherDesignation { get; set; }  
    [Column(TypeName = "jsonb")] public List<FieldTripSupervisor> AdditionalSupervisors { get; set; }
    public int TenantId { get; set; }
}