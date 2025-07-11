using System;
using System.Threading.Tasks;
using RhyoliteOrbit.DomainServices.Payroll.OvertimeTypes.Dto;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.OvertimeTypes;

public interface IOverTimeTypeManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> Create(OvertimeType input);
    Task Update(OvertimeType input);
    Task Delete(Guid id);
    Task<OvertimeType> GetAsync(Guid id);
    Task DeleteOvertimeRate(Guid overtimeTypeId, Guid id);
    Task UpdateOvertimeRate(OvertimeRateDto input);
    Task CreateOvertimeRate(OvertimeRateDto input);
}