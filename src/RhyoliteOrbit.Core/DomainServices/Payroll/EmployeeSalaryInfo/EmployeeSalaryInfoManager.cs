using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeSalaryInfo.Dto;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeSalaryInfo;

public class EmployeeSalaryInfoManager: Abp.Domain.Services.DomainService, IEmployeeSalaryInfoManager
{
        private readonly IRepository<EmployeeSalaryData, Guid> _repositoryEmployeeSalaryData;
        private readonly IRepository<Bank, Guid> _repositoryBank;
        private readonly IRepository<PayrollPeriod, Guid> _repositoryPayrollPeriod;
        private readonly IRepository<PayCalendar, Guid> _repositoryPayCalendar;
        private readonly IRepository<EmployeeDaysWorked, Guid> _repositoryEmployeeDaysWorked;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        
        private readonly IRepository<SalaryIncrementHistory, Guid> _repositorySalaryIncrementHistory;

        public EmployeeSalaryInfoManager(IRepository<EmployeeSalaryData, Guid> repositoryEmployeeSalaryData, IRepository<SalaryIncrementHistory, Guid> repositorySalaryIncrementHistory, IRepository<Bank, Guid> repositoryBank, IRepository<PayrollPeriod, Guid> repositoryPayrollPeriod, IRepository<PayCalendar, Guid> repositoryPayCalendar, IRepository<EmployeeDaysWorked, Guid> repositoryEmployeeDaysWorked, IUnitOfWorkManager unitOfWorkManager)
        {
            _repositoryEmployeeSalaryData = repositoryEmployeeSalaryData;
            _repositorySalaryIncrementHistory = repositorySalaryIncrementHistory;
            _repositoryBank = repositoryBank;
            _repositoryPayrollPeriod = repositoryPayrollPeriod;
            _repositoryPayCalendar = repositoryPayCalendar;
            _repositoryEmployeeDaysWorked = repositoryEmployeeDaysWorked;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task Create(EmployeeSalaryData entity)
        {
            var datta = await _repositoryEmployeeSalaryData.FirstOrDefaultAsync(x => x.EmployeeId == entity.EmployeeId);

            if (datta == null)
            {
                var bankInfo = await _repositoryBank.FirstOrDefaultAsync(entity.BankId);
                if (bankInfo != null)
                {
                    entity.BankName = bankInfo.Name;
                    var branchInfo = bankInfo.BankBranches.FirstOrDefault(x=>x.Id == entity.BankBranchId);

                    if (branchInfo != null)
                    {
                        entity.BankBranchName = branchInfo.Name;
                    }
                }

                await _repositoryEmployeeSalaryData.InsertAsync(entity);
            }
            else
            {
                datta.MonthlySalary = entity.MonthlySalary;
                datta.SalaryType = entity.SalaryType;
                datta.PayType = entity.PayType;
                datta.DailyHours = entity.DailyHours;
                datta.BankId = entity.BankId;
                datta.BankBranchId = entity.BankBranchId;
                datta.AccountNumber = entity.AccountNumber;
                datta.PreviousSalary = entity.PreviousSalary;
                datta.VacationDaysPaid = entity.VacationDaysPaid;
                datta.CurrentHourlyRate = entity.CurrentHourlyRate;
                datta.PaysGraduatedTax = entity.PaysGraduatedTax;
                datta.PaysWithHoldingTax = entity.PaysWithHoldingTax;
                datta.WithHoldingTaxRate = entity.WithHoldingTaxRate;
                
                
                var bankInfo = await _repositoryBank.FirstOrDefaultAsync(entity.BankId);
                if (bankInfo != null)
                {
                    entity.BankName = bankInfo.Name;
                    var branchInfo = bankInfo.BankBranches.FirstOrDefault(x=>x.Id == entity.BankBranchId);

                    if (branchInfo != null)
                    {
                        datta.BankBranchName = branchInfo.Name;
                    }
                }
                
                await _repositoryEmployeeSalaryData.UpdateAsync(datta);
            }
        }

        public async Task Update(EmployeeSalaryData entity)
        {
            await _repositoryEmployeeSalaryData.UpdateAsync(entity);
        }

        public async Task<object> GetAllBySalaryType(string salaryType)
        {

           return await _repositoryEmployeeSalaryData.GetAllListAsync(x=>x.SalaryType == salaryType);

        }

        public async Task<object> GetAllBySalaryType(string salaryType, Guid categoryId)
        {
            return await _repositoryEmployeeSalaryData.GetAllListAsync(x => x.SalaryType == salaryType && x.EmployeeCategoryId == categoryId);

        }

        public async Task<object> GetAllBySalaryGrade(string salaryType, Guid salaryGradeId, Guid salaryNotchId)
        {
            return await _repositoryEmployeeSalaryData.GetAllListAsync(x => x.SalaryType == salaryType && x.EmployeeSalaryGradeId == salaryGradeId && x.EmployeeSalaryNotchId == salaryNotchId);
        }

        public async Task<object> GetAsync(Guid employeeId)
        {
            decimal vacationDaysPaid = 0M;
            string vacationDaySource = string.Empty;

            var currentPayMonth = await (await _repositoryPayrollPeriod.GetAllAsync()).FirstOrDefaultAsync();
 

            if (employeeId != Guid.Empty)
            {
                var employeeSalaryInfo = await _repositoryEmployeeSalaryData.FirstOrDefaultAsync(x => x.EmployeeId == employeeId);

                if (employeeSalaryInfo != null)
                {
                    
                    if (employeeSalaryInfo.VacationDaysPaid > 0)
                    {
                        vacationDaysPaid = employeeSalaryInfo.VacationDaysPaid;
                        vacationDaySource = "Employee Salary Info";
                    }
                    else
                    {
                        if (currentPayMonth != null)
                        {

                            var employeeDaysWorked = await _repositoryEmployeeDaysWorked.FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.Year == currentPayMonth.Year && x.Month == currentPayMonth.Month);
                                
                            if (employeeDaysWorked != null)
                            {
                                vacationDaysPaid = employeeDaysWorked.Days;
                                vacationDaySource = "Employee Days Worked";
                            }
                            else
                            {
                                var payYear = await _repositoryPayCalendar.FirstOrDefaultAsync(x => x.Year == currentPayMonth.Year);

                                var payMonth = payYear.Details.FirstOrDefault(x => x.Year == currentPayMonth.Year && x.Month == currentPayMonth.Month);
                           
                                if (payMonth is { Days: > 0 })
                                {
                                    vacationDaysPaid = payMonth.Days;
                                    vacationDaySource = "Employee Days Worked";
                                }
                            }
                        }
                        
                    }
                    
                    return new
                {
                    employeeSalaryInfo.Id,
                    employeeSalaryInfo.EmployeeId,
                    employeeSalaryInfo.EmployeeName,
                    employeeSalaryInfo.EmployeeIdentifier,
                    employeeSalaryInfo.MonthlySalary,
                    employeeSalaryInfo.PreviousSalary,
                    employeeSalaryInfo.SalaryType,
                    employeeSalaryInfo.EmployeeCategoryId,
                    employeeSalaryInfo.AccountNumber,
                    employeeSalaryInfo.BankId,
                    employeeSalaryInfo.BankName,
                    employeeSalaryInfo.BankBranchId,
                    employeeSalaryInfo.BankBranchName,
                    employeeSalaryInfo.PayType,
                    employeeSalaryInfo.DailyHours,
                    employeeSalaryInfo.CurrentHourlyRate,
                    employeeSalaryInfo.CurrencyId,
                    employeeSalaryInfo.CurrencyName,
                    employeeSalaryInfo.EmployeeSalaryGradeId,
                    employeeSalaryInfo.EmployeeSalaryNotchId,
                    employeeSalaryInfo.PaysGraduatedTax,
                    employeeSalaryInfo.PaysWithHoldingTax,
                    employeeSalaryInfo.WithHoldingTaxRate,
                    employeeSalaryInfo.EmployeePhoto,
                    employeeSalaryInfo.TenantId,
                    vacationDaysPaid,
                    vacationDaySource
                };
                }
            }

            return new { vacationDaysPaid , vacationDaySource, paysGraduatedTax = true, withHoldingTaxRate = false };
        }

        public async Task<object> ListAll(int pageNo, int pageSize, string query)
        {
            var predicate = PredicateBuilder.New<EmployeeSalaryData>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.SalaryType.Contains(query) || x.EmployeeName.Contains(query) || x.EmployeeIdentifier.Contains(query) || x.PayType.Contains(query));
            }
            
            var totalCount =
                
                await _repositoryEmployeeSalaryData.CountAsync(predicate);
            
            var data = await (await _repositoryEmployeeSalaryData.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.EmployeeId, a.EmployeeIdentifier, a.EmployeeName, a.EmployeeSalaryNotchId, a.AccountNumber, a.SalaryType, a.MonthlySalary, a.BankBranchName, a.BankBranchId, a.BankId, a.BankName, a.CurrentHourlyRate, a.EmployeePhoto, a.PaysWithHoldingTax, a.PreviousSalary, a.VacationDaysPaid, a.WithHoldingTaxRate  }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

            var totalPages = totalCount > 0 ? Convert.ToInt32(Math.Ceiling(totalCount / (decimal)pageSize)) : 0;

            var paginatedResult = new
            {
                pageNo,
                totalCount,
                totalPages,
                data,
                lowerBound = pageSize * (pageNo - 1) + 1,
                upperBound = totalPages == pageNo ? totalCount : pageNo * pageSize,
            };

            return paginatedResult;
        }
        
        public async Task Delete(Guid id)
        {
            await _repositoryEmployeeSalaryData.DeleteAsync(id);
        }

        public async Task ProcessSalaryIncrement(List<SalaryIncrement> salaryDataList)
        {
            foreach (var salaryData in salaryDataList)
            {
                var employeeSalaryInfo = await _repositoryEmployeeSalaryData.FirstOrDefaultAsync(x=>x.EmployeeId == salaryData.EmployeeId);

                employeeSalaryInfo.MonthlySalary = salaryData.MonthlySalary;
                employeeSalaryInfo.PreviousSalary= salaryData.PreviousSalary;

                await _repositoryEmployeeSalaryData.UpdateAsync(employeeSalaryInfo);

                var salaryIncrementHistory = new Models.Payroll.SalaryIncrementHistory
                {
                    EmployeeId = salaryData.EmployeeId,
                    EmployeeIdentifier= employeeSalaryInfo.EmployeeIdentifier,
                    EmployeeName= employeeSalaryInfo.EmployeeName,
                    CurrentSalary = salaryData.MonthlySalary,
                    PreviousSalary = salaryData.PreviousSalary,
                    IncrementAmount = salaryData.IncrementAmount,
                };

                await _repositorySalaryIncrementHistory.InsertAsync(salaryIncrementHistory);


            }

        }

        public async Task Restore(List<EmployeeSalaryData> salaryDataList, int tenantId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                foreach (var employeeSalaryInfo in salaryDataList)
                {
                    await _repositoryEmployeeSalaryData.InsertAsync(employeeSalaryInfo);
                }
                
            }

        }
}