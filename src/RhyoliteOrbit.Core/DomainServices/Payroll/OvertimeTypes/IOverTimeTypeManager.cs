using System;
using System.Threading.Tasks;
using RhyoliteOrbit.DomainServices.Payroll.OvertimeTypes.Dto;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.OvertimeTypes;

public interface IOverTimeTypeManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll();
    Task<object> Create(OvertimeType input);
    Task Update(OvertimeType input);
    Task Delete(Guid id);
    Task<OvertimeType> GetAsync(Guid id);
    Task DeleteOvertimeRate(OvertimeRateDto input);
    Task UpdateOvertimeRate(OvertimeRateDto input);
    Task<object> CreateOvertimeRate(OvertimeRateDto input);
}