using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.SchoolManager;

public class Staff: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(20)]
    public string StaffIdentifier { get; set; }
    [MaxLength(350)]
    public string ImageUrl { get; set; }
    [MaxLength(60)]
    public string FirstName { get; set; }
    [MaxLength(20)]
    public string Gender { get; set; }
    [MaxLength(60)]
    public string SocialSecurityNumber { get; set; }
    public DateTime DateEmployed { get; set; }
    public bool IsTeachingStaff { get; set; }
    [MaxLength(60)]
    public string LastName { get; set; }
    [MaxLength(60)]
    public string OtherName { get; set; }
    public DateTime DateOfBirth { get; set; }
    [MaxLength(150)]
    public string HomeAddress { get; set; }
    [MaxLength(60)]
    public string CityOrLocation { get; set; }
    [MaxLength(50)]
    public string MaritalStatus { get; set; }
    [MaxLength(20)]
    public string PrimaryPhone { get; set; }
    [MaxLength(20)]
    public string SecondaryPhone { get; set; }
    [MaxLength(20)]
    public string EmailAddress { get; set; }
    [MaxLength(20)]
    public string EmergencyContactPerson { get; set; }
    [MaxLength(20)]
    public string EmergencyContactPhone { get; set; }
    public Guid NationalityId { get; set; }
    [MaxLength(100)]
    public string NationalityName { get; set; }
    [MaxLength(100)]
    public string Religion { get; set; }
    public int Status { get; set; } //EmployeeStatus enum
    [MaxLength(15)]
    public string Pin { get; set; } 
    public int TenantId { get; set; }
}