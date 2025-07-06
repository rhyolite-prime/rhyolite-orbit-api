using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeBio;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeSalaryInfo;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeSalaryInfo.Dto;
using RhyoliteOrbit.DomainServices.Payroll.SalaryGrades;
using RhyoliteOrbit.DomainServices.Shared.Banks;
using RhyoliteOrbit.DomainServices.Shared.Currencies;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.EmployeeSalaryInfoRecords.Dto;

namespace RhyoliteOrbit.Services.Payroll.EmployeeSalaryInfoRecords;

public class EmployeeSalaryInfoAppService: RhyoliteOrbitAppServiceBase
{
        private readonly IEmployeeSalaryInfoManager _employeeSalaryInfoManager;
        private readonly IEmployeeBioDataManager _employeeBioDataManager;
        private readonly ISalaryGradeManager _salaryGradeManager;
        private readonly IBankManager _bankManager;
        private readonly ICurrencyManager _currencyManager;
        private readonly IMapper _mapper;

        public EmployeeSalaryInfoAppService(IEmployeeSalaryInfoManager employeeSalaryInfoManager, IMapper mapper, IEmployeeBioDataManager employeeBioDataManager, IBankManager bankManager, ICurrencyManager currencyManager)
        {
            _employeeSalaryInfoManager = employeeSalaryInfoManager;
            _mapper = mapper;
            _employeeBioDataManager = employeeBioDataManager;
            _bankManager = bankManager;
            _currencyManager = currencyManager;
        }

        [HttpGet]
        public async Task<object> GetAll(int pageNo, int pageSize, string query)
        {
            return await _employeeSalaryInfoManager.ListAll(pageNo, pageSize, query);
        }

        [HttpGet]
        public async Task<object> GetSalaryInfoBySalaryType(string salaryType)
        {
            return await _employeeSalaryInfoManager.GetAllBySalaryType(salaryType);
        }

        [HttpGet]
        public async Task<object> GetSalaryInfoBySalaryType(string salaryType, Guid categoryId)
        {
            return await _employeeSalaryInfoManager.GetAllBySalaryType(salaryType, categoryId);
        }

        [HttpGet]
        public async Task<object> GetSalaryInfoBySalaryGrade(string salaryType, Guid salaryGradeId, Guid salaryNotchId)
        {
            return await _employeeSalaryInfoManager.GetAllBySalaryGrade(salaryType, salaryGradeId, salaryNotchId);

        }
        
        [HttpGet]
        public async Task<object> GetEmployeeSalaryData(Guid employeeId)
        {
            return await _employeeSalaryInfoManager.GetAsync(employeeId);
        }

        [HttpPost]
        public async Task Create(CreateEmployeeSalaryInfoInput input)
        {
            var obj = _mapper.Map<EmployeeSalaryData>(input);
            await _employeeSalaryInfoManager.Create(obj);
        }


        [HttpPost]
        public async Task Import(List<ImportEmployeeSalaryInfoInput> inputList)
        {

            foreach (var input in inputList)
            {
                var employee = await _employeeBioDataManager.GetAsync(input.EmployeeIdentifier);
                var bankInfo = await _bankManager.GetAsync(input.BankName);
                var currencyInfo = await _currencyManager.GetAsync(input.CurrencyName);
                var salaryGradeInfo = await _salaryGradeManager.GetAsync(input.SalaryGrade);

                var bankBranchId = Guid.Empty;
                var salaryNotchId = Guid.Empty;

                if (bankInfo != null)
                {
                    var branchList = bankInfo.BankBranches;

                    var bankBranchInfo  = branchList.FirstOrDefault(a=> a.Name == input.BankBranch);

                    if (bankBranchInfo != null)
                    {
                        bankBranchId = bankBranchInfo.Id;

                    }

                }


                if(salaryGradeInfo != null) 
                {

                    var salaryNotchList = salaryGradeInfo.SalaryNotches;

                    var salaryNotchInfo = salaryNotchList.FirstOrDefault(x=> x.SalaryGradeName == input.SalaryNotch);

                    if (salaryNotchInfo != null)
                    {

                        salaryNotchId = salaryNotchInfo.Id;
                    }

                }


                if (employee != null) 
                {
                    var employeeSalaryInfo = new CreateEmployeeSalaryInfoInput
                    {
                        EmployeeIdentifier = employee.EmployeeIdentifier,
                        EmployeeName = string.IsNullOrEmpty(employee.OtherName) ? $"{employee.LastName} {employee.FirstName}" : $"{employee.LastName} {employee.FirstName} {employee.OtherName}",
                        SalaryType = input.SalaryType,
                        EmployeeCategoryId = employee.CategoryId,
                        EmployeeId = employee.Id,
                        AccountNumber = input.AccountNumber,
                        BankBranchId = bankBranchId,
                        PayType = input.PayType,
                        MonthlySalary = input.MonthlySalary,
                        PreviousSalary = input.PreviousSalary,
                        BankId = bankInfo?.Id ?? Guid.Empty,
                        CurrencyId = currencyInfo?.Id ?? Guid.Empty,
                        CurrencyName = currencyInfo != null ? currencyInfo.Name : string.Empty,
                        CurrentHourlyRate = input.CurrentHourlyRate,
                        DailyHours = input.DailyHours,
                        EmployeePhoto = string.Empty,
                        EmployeeSalaryGradeId = salaryGradeInfo?.Id ?? Guid.Empty,
                        EmployeeSalaryNotchId = salaryNotchId,
                        VacationDaysPaid = input.VacationDaysPaid,

                    };


                    var obj = _mapper.Map<EmployeeSalaryData>(employeeSalaryInfo);
                    await _employeeSalaryInfoManager.Create(obj);
                }

            }
            
        }
        
        [HttpPost]
        public async Task ProcessSalaryIncrement(List<SalaryIncrement> salaryDataList)
        {
             await _employeeSalaryInfoManager.ProcessSalaryIncrement(salaryDataList);
        }

        [HttpPost]
        public async Task Update(UpdateEmployeeSalaryInfoInput input)
        {
            var obj = _mapper.Map<EmployeeSalaryData>(input);
            var tenantInfo = await GetCurrentTenantAsync();
            obj.TenantId = tenantInfo.Id;
            await _employeeSalaryInfoManager.Update(obj);
        }

        [HttpDelete]
        public async Task Delete(Guid id)
        {
            await _employeeSalaryInfoManager.Delete(id);
        }

}