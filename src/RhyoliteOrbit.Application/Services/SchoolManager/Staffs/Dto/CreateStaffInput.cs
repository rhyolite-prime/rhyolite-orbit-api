using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.Staffs.Dto;

public class CreateStaffInput
{
    public string StaffIdentifier { get; set; }
    public string ImageUrl { get; set; }
    public string FirstName { get; set; }
    public string Gender { get; set; }
    public string SocialSecurityNumber { get; set; }
    public DateTime DateEmployed { get; set; }
    public bool IsTeachingStaff { get; set; }
    public string LastName { get; set; }
    public string OtherName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string HomeAddress { get; set; }
    public string CityOrLocation { get; set; }
    public string MaritalStatus { get; set; }
    public string PrimaryPhone { get; set; }
    public string SecondaryPhone { get; set; }
    public string EmailAddress { get; set; }
    public string EmergencyContactPerson { get; set; }
    public string EmergencyContactPhone { get; set; }
    public string Nationality { get; set; }
    public RhyoliteOrbitConsts.EmployeeStatus Status { get; set; } = (int)RhyoliteOrbitConsts.EmployeeStatus.Active;
}

public class UpdateStaffInput: EntityDto<Guid>
{
    public string StaffIdentifier { get; set; }
    public string ImageUrl { get; set; }
    public string FirstName { get; set; }
    public string Gender { get; set; }
    public string SocialSecurityNumber { get; set; }
    public DateTime DateEmployed { get; set; }
    public bool IsTeachingStaff { get; set; }
    public string LastName { get; set; }
    public string OtherName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string HomeAddress { get; set; }
    public string CityOrLocation { get; set; }
    public string MaritalStatus { get; set; }
    public string PrimaryPhone { get; set; }
    public string SecondaryPhone { get; set; }
    public string EmailAddress { get; set; }
    public string EmergencyContactPerson { get; set; }
    public string EmergencyContactPhone { get; set; }
    public string Nationality { get; set; }
    public RhyoliteOrbitConsts.EmployeeStatus Status { get; set; }
}