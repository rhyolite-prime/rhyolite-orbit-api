using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.Students.Dto;

public class CreateStudentInput
{
    public string StudentIdentifier { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Guid NationalityId { get; set; } = Guid.Empty;
    public string NationalityName { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public string FirstName { get; set; }
    public string Gender { get; set; }
    public Guid ReligionId { get; set; }  = Guid.Empty;
    public string ReligionName { get; set; }
    public Guid ClassId { get; set; }  = Guid.Empty;
    public string ClassName { get; set; }
    public RhyoliteOrbitConsts.StudentStatus Status { get; set; }
    public Guid AcademicYearId { get; set; }  = Guid.Empty;
    public string AcademicYearName { get; set; }
    public string HomeAddress { get; set; }
    public string CityOrLocation { get; set; }
    public string ImageUrl { get; set; }
    public string EnrollmentType { get; set; }
    public string BiometricIdentifier { get; set; }
    public bool IsAlumni { get; set; }
    public DateTime CompletionDate { get; set; }
}

public class UpdateStudentInput: EntityDto<Guid>
{
    public string StudentIdentifier { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Guid NationalityId { get; set; } = Guid.Empty;
    public string NationalityName { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public string FirstName { get; set; }
    public string Gender { get; set; }
    public Guid ReligionId { get; set; } = Guid.Empty;
    public string ReligionName { get; set; }
    public Guid ClassId { get; set; } = Guid.Empty;
    public string ClassName { get; set; }
    public RhyoliteOrbitConsts.StudentStatus Status { get; set; }
    public Guid AcademicYearId { get; set; }  = Guid.Empty;
    public string AcademicYearName { get; set; }
    public string HomeAddress { get; set; }
    public string CityOrLocation { get; set; }
    public string ImageUrl { get; set; }
    public string EnrollmentType { get; set; }
    public string BiometricIdentifier { get; set; }
    public bool IsAlumni { get; set; }
    public DateTime CompletionDate { get; set; }
}