using System;
using System.Threading.Tasks;
using RhyoliteOrbit.DomainServices.Payroll.SalaryGrades.Dto;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.SalaryGrades;

public interface ISalaryGradeManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> Create(SalaryGrade input);
    Task<SalaryGrade> GetAsync(string name);
    Task Update(SalaryGrade input);
    Task Delete(Guid id);
    Task<object> CreateSalaryNotch(SalaryNotchDto input);
    Task UpdateSalaryNotch(SalaryNotchDto input);
    Task DeleteSalaryNotch(SalaryNotchDto input);
}