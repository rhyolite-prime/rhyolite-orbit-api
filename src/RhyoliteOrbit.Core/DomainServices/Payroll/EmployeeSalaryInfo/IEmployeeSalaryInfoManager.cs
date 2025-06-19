using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeSalaryInfo.Dto;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeSalaryInfo;

public interface IEmployeeSalaryInfoManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll();
    Task<object> GetAsync(Guid employeeId);
    Task<object> GetAllBySalaryType(string salaryType);
    Task<object> GetAllBySalaryType(string salaryType, Guid categoryId);
    Task<object> GetAllBySalaryGrade(string salaryType, Guid salaryGradeId, Guid salaryNotchId);
    Task ProcessSalaryIncrement(List<SalaryIncrement> salaryDataList);
    Task Create(EmployeeSalaryData input);
    Task Update(EmployeeSalaryData input);
    Task Delete(Guid id);
    Task Restore(List<EmployeeSalaryData> salaryDataList, int tenantId);
}