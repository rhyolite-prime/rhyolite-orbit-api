using System;
using System.Threading.Tasks;
using RhyoliteOrbit.DomainServices.Payroll.AllowanceTypes.Dto;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.AllowanceTypes;

public interface IAllowanceTypeManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> ListAll(Guid categoryId);
    Task<object> ListAll(Guid allowanceTypeId, Guid employeeId, Guid categoryId);
    Task<object> Create(AllowanceType input);
    Task<AllowanceType> GetAsync(Guid id);
    Task<AllowanceType> GetAsync(string name);
    Task Update(AllowanceType input);
    Task Delete(Guid id);
    //rates
    Task CreateAllowanceRate(AllowanceRateDto input);
    Task UpdateAllowanceRate(AllowanceRateDto input);
    Task DeleteAllowanceRate(Guid allowanceTypeId,  Guid id);
}