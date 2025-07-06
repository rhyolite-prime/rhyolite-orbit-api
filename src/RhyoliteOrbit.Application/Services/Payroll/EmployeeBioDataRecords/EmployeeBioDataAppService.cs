using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeBio;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeBio.Dto;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeCategories;
using RhyoliteOrbit.DomainServices.Payroll.SalaryGrades;
using RhyoliteOrbit.DomainServices.Shared.Countries;
using RhyoliteOrbit.DomainServices.Shared.Departments;
using RhyoliteOrbit.DomainServices.Shared.PersonnelStatuses;
using RhyoliteOrbit.DomainServices.Shared.Religions;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.EmployeeBioDataRecords.Dto;

namespace RhyoliteOrbit.Services.Payroll.EmployeeBioDataRecords;

public class EmployeeBioDataAppService: RhyoliteOrbitAppServiceBase
{
        private readonly IEmployeeBioDataManager _employeeBioDataManager;
        private readonly IEmployeeCategoryManager _employeeCategoryManager;
        private readonly IDepartmentManager _departmentManager;
        private readonly IReligionManager _religionManager;
        private readonly ISalaryGradeManager _salaryGradeManager;
        private readonly IPersonnelStatusManager _personnelStatusManager;
        private readonly ICountryManager _countryManager;
        private readonly IMapper _mapper;

        public EmployeeBioDataAppService(IEmployeeBioDataManager employeeBioDataManager, IMapper mapper, IEmployeeCategoryManager employeeCategoryManager, IDepartmentManager departmentManager, IReligionManager religionManager, IPersonnelStatusManager personnelStatusManager, ICountryManager countryManager, ISalaryGradeManager salaryGradeManager)
        {
            _employeeBioDataManager = employeeBioDataManager;
            _mapper = mapper;
            _employeeCategoryManager = employeeCategoryManager;
            _departmentManager = departmentManager;
            _religionManager = religionManager;
            _personnelStatusManager = personnelStatusManager;
            _countryManager = countryManager;
            _salaryGradeManager = salaryGradeManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetAll(int pageNo, int pageSize, string query)
        {
            return await _employeeBioDataManager.ListAll(pageNo, pageSize, query);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> Create(CreateEmployeeBioDataInput input)
        {
            var obj = _mapper.Map<EmployeeBioData>(input);
            return await _employeeBioDataManager.Create(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputList"></param>
        [HttpPost]
        public async Task Import(List<ImportEmployeeBioDataInput> inputList)
        {
            foreach (var input in inputList)
            {
                var categoryInfo = await _employeeCategoryManager.GetAsync(input.CategoryName);
                var departmentInfo = await _departmentManager.GetAsync(input.DepartmentName);
                var religionInfo = await _religionManager.GetAsync(input.ReligionName);
                var salaryGradeInfo = await _salaryGradeManager.GetAsync(input.SalaryGradeName);
                var statusInfo = await _personnelStatusManager.GetAsync(input.StatusName);
                var nationalityInfo = await _countryManager.GetNationality(input.NationalityName);

                var salaryNotchId = Guid.Empty;

                if (salaryGradeInfo != null)
                {
                    var salaryNotchList = salaryGradeInfo.SalaryNotches;

                    var salaryNotchInfo = salaryNotchList.FirstOrDefault(x=> x.Notch == input.SalaryNotchName);

                    if(salaryNotchInfo != null)
                    {
                        salaryNotchId = salaryNotchInfo.Id;
                    }

                }

                var employeeBioData = new CreateEmployeeBioDataInput
                {
                     CategoryId= categoryInfo?.Id ?? Guid.Empty,
                     CategoryName= input.CategoryName,
                     CityOrLocation = input.CityOrLocation,
                     DepartmentId = departmentInfo?.Id ?? Guid.Empty,
                     DepartmentName= input.DepartmentName,
                     ReligionId = religionInfo?.Id ?? Guid.Empty,
                     ReligionName = input.ReligionName,
                     CompanyEmail= input.CompanyEmail,
                     DateAppointed= input.DateAppointed,
                     DateOfBirth= input.DateOfBirth,
                     DriverLicenseNo = input.DriverLicenseNo,
                     EmployeeIdentifier = input.EmployeeIdentifier,
                     EmployeePhoto = input.EmployeePhoto, 
                     FirstName = input.FirstName, 
                     Gender= input.Gender, 
                     LastName = input.LastName,
                     MaritalStatus= input.MaritalStatus, 
                     OtherName = input.OtherName, 
                     PersonalEmail = input.PersonalEmail, 
                     ResidenceAddress = input.ResidenceAddress, 
                     NationalHealthInsuranceNo = input.NationalID,
                     NationalityId = nationalityInfo?.Id ?? Guid.Empty,
                     StatusId = statusInfo?.Id ?? Guid.Empty,
                     StatusName= input.StatusName,
                     Hometown = input.Hometown,
                     TaxIdentificationNo = input.TaxIdentificationNo, 
                     PrimaryPhoneNumber = input.PrimaryPhoneNumber,
                     SalaryGradeId = salaryGradeInfo?.Id ?? Guid.Empty,
                     SalaryGradeName = input.SalaryGradeName,
                     SalaryNotchId = salaryNotchId,
                     SecondaryPhoneNumber= input.SecondaryPhoneNumber, 
                     SalaryNotchName = input.SalaryNotchName, 
                     NationalIdentificationNo = input.NationalIdentificationNo,
                     NationalityName = input.NationalityName,
                     TitleName = input.TitleName,
                     UserId = 0, Height = 0, 
                     Weight = 0, LeaveDaysEntitled = 0, 
                     MedicalExpensesLimit = 0,
                };
                var obj = _mapper.Map<EmployeeBioData>(employeeBioData);
                await _employeeBioDataManager.Create(obj);
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="input"></param>
        public async Task Update(UpdateEmployeeBioDataInput input)
        {
            var obj = _mapper.Map<EmployeeBioData>(input);
            await _employeeBioDataManager.Update(obj);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete]
        public async Task Delete(Guid id)
        {
            await _employeeBioDataManager.Delete(id);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="statusId"></param>
        [HttpGet]
        public async Task UpdateStatus(Guid employeeId, Guid statusId)
        {
            await _employeeBioDataManager.UpdateStatus(employeeId, statusId);
        }

        // export ...
        [HttpGet]
        public async Task<object> ExportAllowances()
        {
            return await _employeeBioDataManager.ExportAllowances();
        }

        [HttpGet]
        public async Task<object> ExportBenefitsInKind()
        {
            return await _employeeBioDataManager.ExportBenefitsInKind();
        }

        [HttpGet]
        public async Task<object> ExportSalaryInfo()
        {
            return await _employeeBioDataManager.ExportSalaryInfo();
        }

        [HttpGet]
        public async Task<object> ExportSsnit()
        {
            return await _employeeBioDataManager.ExportSsnit();
        }

        [HttpGet]
        public async Task<object> ExportDeductions()
        {
            return await _employeeBioDataManager.ExportDeductions();

        }

        [HttpGet]
        public async Task<object> ExportEmployeeLoans()
        {
            return await _employeeBioDataManager.ExportEmployeeLoans();

        }
}