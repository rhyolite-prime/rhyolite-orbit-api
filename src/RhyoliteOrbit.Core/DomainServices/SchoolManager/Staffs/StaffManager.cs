using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.SchoolManager.Staffs;

public class StaffManager: Abp.Domain.Services.DomainService, IStaffManager
{
        private readonly IRepository<Staff, Guid> _repositoryStaff;
        private readonly IRepository<EmployeeBioData, Guid> _repositoryEmployeeBioData;
        private readonly IRepository<Country, Guid> _repositoryCountry;
        private readonly IRepository<SystemNumber, Guid> _repositorySystemNumber;
        public StaffManager(IRepository<Staff, Guid> repositoryStaff, IRepository<EmployeeBioData, Guid> repositoryEmployeeBioData, IRepository<Country, Guid> repositoryCountry, IRepository<SystemNumber, Guid> repositorySystemNumber)
        {
            _repositoryStaff = repositoryStaff;
            _repositoryEmployeeBioData = repositoryEmployeeBioData;
            _repositoryCountry = repositoryCountry;
            _repositorySystemNumber = repositorySystemNumber;
        }


        public async Task Create(Staff entity)
        {
            var datta = await _repositoryStaff.FirstOrDefaultAsync(x => x.StaffIdentifier == entity.StaffIdentifier);
            
            if (datta == null)
            {
                var countryInfo = await _repositoryCountry.FirstOrDefaultAsync(x=>x.Id == entity.NationalityId);

                if (countryInfo != null)
                {
                    entity.NationalityName = countryInfo.Nationality;
                }

                var staffEntryResult = await _repositoryStaff.InsertAsync(entity);

                // create as employee in Payroll.
                var employee = new EmployeeBioData
                {
                    EmployeeIdentifier = entity.StaffIdentifier,
                    FirstName = entity.FirstName,
                    OtherName = entity.OtherName,
                    LastName = entity.LastName,
                    EmployeePhoto = entity.ImageUrl,
                    Gender = entity.Gender,
                    Status = entity.Status,
                    CityOrLocation = entity.CityOrLocation,
                    DepartmentId = Guid.Empty,
                    CategoryId = Guid.Empty,
                    DateAppointed = entity.DateEmployed,
                    SalaryGradeId = Guid.Empty,
                    SalaryNotchId = Guid.Empty,
                    TaxIdentificationNo = "",
                    DateOfBirth = entity.DateOfBirth,
                    PersonalEmail = entity.EmailAddress,
                    MaritalStatus = entity.MaritalStatus,
                    ReligionId = Guid.Empty,
                    NationalityId = entity.NationalityId,
                    Hometown = "",
                    PrimaryPhoneNumber = entity.PrimaryPhone,
                    SecondaryPhoneNumber = entity.SecondaryPhone,
                    CompanyEmail = entity.EmailAddress,
                    ResidenceAddress = entity.HomeAddress,
                    TenantId = entity.TenantId,
                    NationalIdentificationNo = "",
                    NationalHealthInsuranceNo = "",
                    LicenseIssueDate = null,
                    DriverLicenseNo = null,
                    LicenseExpiryDate = null,
                    IdentityNo = string.Empty,
                    IdentityType = string.Empty,
                    Height = 0,
                    Weight = 0,
                    Languages = null,
                    Interests = null,
                    HealthIssues = null,
                    LeaveDaysEntitled = 15,
                    MedicalExpensesLimit = 0,
                    UserId = 0,
                    Id = staffEntryResult.Id
                };
                await _repositoryEmployeeBioData.InsertAsync(employee);

                return;
            }

            throw new Abp.UI.UserFriendlyException(400, "Duplicate Staff ID's are not allowed.");
            
        }


        public async Task RegisterViaSelfService(Staff entity)
        {
            var employeeId = Guid.NewGuid();
            
            //generate staff Id
            var systemNumber = await _repositorySystemNumber.FirstOrDefaultAsync(x => x.Module == (int)RhyoliteOrbitConsts.ErpModules.SchoolInformationSystem && x.ItemName == "StaffID");
        
            // Increment the last number
            systemNumber.LastNo++;
            
            entity.Id = employeeId;
            
            //assign the staff identifier using the specified format from the configured system number...
            
            // Use FormatPattern if defined, otherwise default to [Prefix]-[Number]-[Suffix]
            var format = string.IsNullOrWhiteSpace(systemNumber.FormatPattern)
                ? "{prefix}-{number}-{suffix}"
                : systemNumber.FormatPattern;
            
            entity.StaffIdentifier = format
                .Replace("{prefix}", systemNumber.Prefix ?? string.Empty)
                .Replace("{number}", systemNumber.LastNo.ToString("D4")) // padded number e.g. 0001
                .Replace("{suffix}", systemNumber.Suffix ?? string.Empty);
            
            await _repositoryStaff.InsertAsync(entity);

            // create as employee in Payroll.
            var employee = new EmployeeBioData
            {
                EmployeeIdentifier = entity.StaffIdentifier,
                FirstName = entity.FirstName,
                OtherName = entity.OtherName,
                LastName = entity.LastName,
                EmployeePhoto = entity.ImageUrl,
                Gender = entity.Gender,
                Status = entity.Status,
                CityOrLocation = entity.CityOrLocation,
                DepartmentId = Guid.Empty,
                CategoryId = Guid.Empty,
                DateAppointed = entity.DateEmployed,
                SalaryGradeId = Guid.Empty,
                SalaryNotchId = Guid.Empty,
                TaxIdentificationNo = "",
                DateOfBirth = entity.DateOfBirth,
                PersonalEmail = entity.EmailAddress,
                MaritalStatus = entity.MaritalStatus,
                ReligionId = Guid.Empty,
                NationalityId = entity.NationalityId,
                Hometown = "",
                PrimaryPhoneNumber = entity.PrimaryPhone,
                SecondaryPhoneNumber = entity.SecondaryPhone,
                CompanyEmail = entity.EmailAddress,
                ResidenceAddress = entity.HomeAddress,
                TenantId = entity.TenantId,
                NationalIdentificationNo = "",
                NationalHealthInsuranceNo = "",
                LicenseIssueDate = null,
                DriverLicenseNo = null,
                LicenseExpiryDate = null,
                IdentityNo = string.Empty,
                IdentityType = string.Empty,
                Height = 0,
                Weight = 0,
                Languages = null,
                Interests = null,
                HealthIssues = null,
                LeaveDaysEntitled = 15,
                MedicalExpensesLimit = 0,
                UserId = 0,
                Id = employeeId
            };
            await _repositoryEmployeeBioData.InsertAsync(employee);
            
            // Save updated last number
            await _repositorySystemNumber.UpdateAsync(systemNumber);
            
        }
        public async Task<object> ListAll(int pageNo, int pageSize, string query)
        {
            var predicate = PredicateBuilder.New<Staff>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.LastName.ToLower().Contains(query.ToLower()));
            }
            
            var totalCount =
                
                await _repositoryStaff.CountAsync(predicate);
            
            var data = await (await _repositoryStaff.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.LastName, a.FirstName, a.OtherName, a.DateEmployed, a.EmailAddress, a.PrimaryPhone, a.EmergencyContactPerson, a.EmergencyContactPhone, a.CreationTime, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

            var totalPages = totalCount > 0 ? Convert.ToInt32(Math.Ceiling(totalCount / (decimal)pageSize)) : 0;

            var paginatedResult = new
            {
                pageNo,
                pageSize,
                totalCount,
                totalPages,
                data,
                lowerBound = pageSize * (pageNo - 1) + 1,
                upperBound = totalPages == pageNo ? totalCount : pageNo * pageSize,
            };

            return paginatedResult;
        }
        
        public async Task<object> ListAll(bool isTeachingStaff)
        {
            return await _repositoryStaff.GetAllListAsync(x=>x.IsTeachingStaff == isTeachingStaff);
        }

        public async Task<object> GetAsync(Guid id)
        {
            return await _repositoryStaff.GetAsync(id);
        }
        public async Task Delete(Guid id)
        {
            await _repositoryStaff.DeleteAsync(id);

            //delete from employees
            await _repositoryEmployeeBioData.DeleteAsync(id);
        }

        public async Task Update(Staff entity)
        {

            var countryInfo = await _repositoryCountry.FirstOrDefaultAsync(x => x.Id == entity.NationalityId);
            
            if (countryInfo != null)
            {
                entity.NationalityName = countryInfo.Nationality;
            }

            await _repositoryStaff.UpdateAsync(entity);

            // update as employee in Payroll
            var payrollEmployee = await _repositoryEmployeeBioData.FirstOrDefaultAsync(x => x.EmployeeIdentifier == entity.StaffIdentifier);
            payrollEmployee.NationalityName = entity.NationalityName;
            payrollEmployee.FirstName = entity.FirstName;
            payrollEmployee.OtherName = entity.OtherName;
            payrollEmployee.LastName = entity.LastName;
            await _repositoryEmployeeBioData.UpdateAsync(payrollEmployee);


        }
    
    
}