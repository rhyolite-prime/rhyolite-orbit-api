using System;
using System.Threading.Tasks;
using RhyoliteOrbit.DomainServices.Payroll.BikTypes.Dto;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.BikTypes;

public interface IBikTypeManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> GetAsync(Guid id);
    Task<object> ListAll(Guid bikTypeId, Guid employeeId, Guid categoryId);
    Task Create(BikType entity);
    Task CreateBikRate(BikRateDto dto);
    Task<BikType> GetAsync(string name);
    Task UpdateBikRate(BikRateDto dto);
    Task DeleteBikRate(Guid bikTypeId, Guid id);
    Task Update(BikType entity);
    Task Delete(Guid id);
}