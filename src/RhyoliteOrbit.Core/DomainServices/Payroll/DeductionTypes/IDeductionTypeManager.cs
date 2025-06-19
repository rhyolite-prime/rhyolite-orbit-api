using System;
using System.Threading.Tasks;
using RhyoliteOrbit.DomainServices.Payroll.DeductionTypes.Dto;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.DeductionTypes;

public interface IDeductionTypeManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> ListAll(Guid categoryId);
    Task<DeductionType> GetAsync(string name);
    Task<object> ListAll(Guid deductionTypeId, Guid employeeId, Guid categoryId);
    Task<object> Create(DeductionType input);
    Task Update(DeductionType input);
    Task Delete(Guid id);
    //
    Task DeleteDeductionRate(DeductionRateDto input);
    Task UpdateDeductionRate(DeductionRateDto input);
    Task<object> CreateDeductionRate(DeductionRateDto input);
}