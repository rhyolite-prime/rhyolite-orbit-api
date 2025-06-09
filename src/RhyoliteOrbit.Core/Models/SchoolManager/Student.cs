using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.SchoolManager;

public class Student: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(20)]
    public string StudentIdentifier { get; set; }
   
    [MaxLength(60)]
    public string LastName { get; set; }
    [MaxLength(60)]
    public string MiddleName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Guid NationalityId { get; set; }
    [MaxLength(100)]
    public string NationalityName { get; set; }
    public DateTime EnrollmentDate { get; set; }
    [MaxLength(60)]
    public string FirstName { get; set; }
    [MaxLength(10)]
    public string Gender { get; set; }
    public Guid ReligionId { get; set; }
    [MaxLength(100)]
    public string ReligionName { get; set; }
    public Guid ClassId { get; set; }
    [MaxLength(100)]
    public string ClassName { get; set; }
    public Guid AcademicYearId { get; set; }
    [MaxLength(100)]
    public string AcademicYearName { get; set; }
    [MaxLength(150)]
    public string HomeAddress { get; set; }
    [MaxLength(150)]
    public string CityOrLocation { get; set; }
    [MaxLength(350)]
    public string ImageUrl { get; set; }
    public Guid StudentStatusId { get; set; }
    [MaxLength(350)]
    public string StudentStatusName { get; set; }
    [MaxLength(50)]
    public string EnrollmentType { get; set; }
    [MaxLength(100)]
    public string BiometricIdentifier { get; set; }
    public bool IsAlumni { get; set; }
    public DateTime CompletionDate { get; set; }
    public int TenantId { get; set; }
}