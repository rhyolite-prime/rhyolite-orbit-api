using System;
using System.Threading.Tasks;
using RhyoliteOrbit.DomainServices.Payroll.BikTypes.Dto;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.BikTypes;

public interface IBikTypeManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> ListAll(Guid bikTypeId, Guid employeeId, Guid categoryId);
    Task<object> Create(BikType entity);
    Task<object> CreateBikRate(BikRateDto dto);
    Task<BikType> GetAsync(string name);
    Task UpdateBikRate(BikRateDto dto);
    Task DeleteBikRate(BikRateDto dto);
    Task Update(BikType entity);
    Task Delete(Guid id);
}