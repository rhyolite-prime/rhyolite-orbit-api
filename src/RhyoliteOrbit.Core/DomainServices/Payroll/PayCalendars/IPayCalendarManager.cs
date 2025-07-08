using System;
using System.Threading.Tasks;
using RhyoliteOrbit.DomainServices.Payroll.PayCalendars.Dto;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.PayCalendars;

public interface IPayCalendarManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll();
    Task Create(PayCalendar input);
    Task Update(PayCalendar input);
    Task Delete(Guid id);
    Task<object> GetDefaultAllowanceDays();
    Task<object> CreatePayCalendarDetails(PayCalendarDetailDto input);
    Task UpdatePayCalendarDetails(PayCalendarDetailDto input);
    Task DeletePayCalendarDetails(PayCalendarDetailDto input);
}