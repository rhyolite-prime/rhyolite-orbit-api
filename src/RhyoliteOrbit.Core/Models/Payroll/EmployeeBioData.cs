using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Payroll;

public class EmployeeBioData: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
        // employee info
        [MaxLength(20)]
        public string EmployeeIdentifier { get; set; }
        [MaxLength(100)]
        public string FirstName { get; set; }
        [MaxLength(100)]
        public string OtherName { get; set; }
        [MaxLength(100)]
        public string LastName { get; set; }
        [MaxLength(350)]
        public string EmployeePhoto { get; set; }
        [MaxLength(50)]
        
        public string Title { get; set; }
        [MaxLength(10)]
        public string Gender { get; set; }
        public int Status { get; set; }
        [MaxLength(100)]
        public string CityOrLocation { get; set; }
        [MaxLength(150)]
        public string ResidenceAddress { get; set; }
        public Guid DepartmentId { get; set; }
        [MaxLength(150)]
        public string DepartmentName { get; set; }
        public Guid CategoryId { get; set; }
        [MaxLength(150)]
        public string CategoryName { get; set; }
        public DateTime DateAppointed { get; set; }
        public Guid SalaryGradeId { get; set; }
        [MaxLength(150)]
        public string SalaryGradeName { get; set; }
        public Guid SalaryNotchId { get; set; }
        [MaxLength(150)]
        public string SalaryNotchName { get; set; }
        
        public Guid RankId { get; set; }
        [MaxLength(150)]
        
        public string RankName { get; set; }
        [MaxLength(20)]
        public string TaxIdentificationNo { get; set; }

        // personal info
        public DateTime DateOfBirth { get; set; }
        [MaxLength(100)]
        public string PersonalEmail { get; set; }
        [MaxLength(20)]
        public string MaritalStatus { get; set; }
        
        [MaxLength(100)]
        public string Religion { get; set; }
        public Guid NationalityId { get; set; }
        [MaxLength(100)]
        public string NationalityName { get; set; }
        [MaxLength(100)]
        public string Hometown { get; set; }
        // contact info
        [MaxLength(20)]
        public string PrimaryPhoneNumber { get; set; }
        [MaxLength(20)]
        public string SecondaryPhoneNumber { get; set; }
        [MaxLength(100)]
        public string CompanyEmail { get; set; }

        //HR related...
        [MaxLength(20)]
        public string NationalIdentificationNo { get; set; }
        [MaxLength(20)]
        public string NationalHealthInsuranceNo { get; set; }
        
        [MaxLength(20)]
        public string DriverLicenseNo { get; set; }
        public DateTime? DriverLicenseIssueDate { get; set; }
        public DateTime? DriverLicenseExpiryDate { get; set; }
        
        [MaxLength(50)]
        public string IdentityType { get; set; }
        [MaxLength(20)]
        public string IdentityNo { get; set; }
        public decimal Height { get; set; } //in meters
        public decimal Weight { get; set; } // in kg
        [Column(TypeName = "jsonb")] public List<string> Languages { get; set; }
        [Column(TypeName = "jsonb")] public List<string> Interests { get; set; }
        [Column(TypeName = "jsonb")] public List<string> HealthIssues { get; set; }
        public int LeaveDaysEntitled { get; set; }

        //medical expenses settings ...
        public decimal MedicalExpensesLimit { get; set; }
        //branch details
        public int UserId { get; set; }
        public int TenantId { get; set; }
    
}