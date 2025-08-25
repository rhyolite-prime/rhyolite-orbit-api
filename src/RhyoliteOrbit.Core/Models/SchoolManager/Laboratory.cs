using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.Models.SchoolManager;

public class Laboratory: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string Location { get; set; }
    
    public int Capacity { get; set; }
    
    public int Status { get; set; }
    
    [Column(TypeName = "jsonb")] public List<LaboratoryEquipment> EquipmentList { get; set; }
    
    public Guid ManagerId { get; set; } // Reference to Staff/employee entity
    
    public string ManagerName { get; set; }
    
    public string ContactPhone { get; set; }
    
    public string ContactEmail { get; set; }
    
    public Guid ManagerDepartmentId { get; set; }
    
    public string ManagerDepartmentName { get; set; }
    
    [Column(TypeName = "jsonb")] public List<string> RequiredCertifications { get; set; }
    
    public DateTime? LastSafetyInspection { get; set; }
    
    public DateTime? SafetyCertificationExpiry { get; set; }
    
    public int SafetyLevel { get; set; } // 1-5 or similar scale
    
    public string BuildingName { get; set; }
    
    public string Floor { get; set; }
    
    public string RoomNumber { get; set; }
    
    [Column(TypeName = "jsonb")] public List<OperatingHour> OperatingHours { get; set; }
    
    [Column(TypeName = "jsonb")] public List<string> AccessRequirements { get; set; }
    
    public int TenantId { get; set; }
}