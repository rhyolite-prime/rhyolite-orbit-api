using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.DomainServices.Payroll.PayCalendars.Dto;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.PayCalendars;

public class PayCalendarManager: Abp.Domain.Services.DomainService, IPayCalendarManager
{
        private readonly IRepository<PayCalendar, Guid> _repositoryPayCalendar;
        private readonly IRepository<InitializePayMonth, Guid> _repositoryInitializePayMonth;

        public PayCalendarManager(IRepository<PayCalendar, Guid> repositoryPayCalendar, IRepository<InitializePayMonth, Guid> repositoryInitializePayMonth)
        {
            _repositoryPayCalendar = repositoryPayCalendar;
            _repositoryInitializePayMonth = repositoryInitializePayMonth;
        }

        public async Task Create(PayCalendar entity)
        {
            await _repositoryPayCalendar.InsertAsync(entity);

        }

        public async Task<object> CreatePayCalendarDetails(PayCalendarDetailDto dto)
        {
            var payCalendarData = await _repositoryPayCalendar.GetAsync(dto.PayCalendarId);
            var calendarList = payCalendarData.Details;
            var calendarInfo = calendarList.FirstOrDefault(x => x.Id == dto.Id);

            if (calendarInfo == null)
            {
                
                var existingPayCalendarDetail =
                    payCalendarData.Details.FirstOrDefault(x => x.Month == dto.Month && x.PayCalendarId == dto.PayCalendarId);

                if (existingPayCalendarDetail != null)
                {
                    throw new UserFriendlyException(
                        400,
                        "Duplicate records are not allowed.",
                        "An entry for the same pay month already exists in the pay calendar."
                    );
                    
                    
                }
                
                
                payCalendarData.Details.Add(new PayCalendarDetail
                {
                    Id = dto.Id,
                    PayCalendarId= dto.PayCalendarId,
                    Year= payCalendarData.Year, 
                    Days = dto.Days, 
                    Month= dto.Month,

                });

                await _repositoryPayCalendar.UpdateAsync(payCalendarData);

                return new
                {
                    code = 200,
                    message = "successful"
                };
            }
            
            throw new UserFriendlyException(
                400,
                "Duplicate records are not allowed.",
                "An entry for the same month already exists in the pay calendar."
            );
            
        }


        public async Task Update(PayCalendar entity)
        {
            await _repositoryPayCalendar.UpdateAsync(entity);
        }

        public async Task UpdatePayCalendarDetails(PayCalendarDetailDto input)
        {
            var payCalendarData = await _repositoryPayCalendar.GetAsync(input.PayCalendarId);
            var calendarList = payCalendarData.Details;
            var calendarInfo = calendarList.FirstOrDefault(x => x.Id == input.Id);

            calendarList.Remove(calendarInfo);

            if (calendarInfo != null)
            {
                calendarInfo.Year = payCalendarData.Year;
                calendarInfo.Days = input.Days;
                calendarInfo.Month = input.Month;

                calendarList.Add(calendarInfo);
            }

            payCalendarData.Details = calendarList;

            await _repositoryPayCalendar.UpdateAsync(payCalendarData);
        }


        public async Task<object> GetDefaultAllowanceDays()
        {
            var currentPayMonth =  await (await _repositoryInitializePayMonth.GetAllAsync()).FirstOrDefaultAsync();

            if (currentPayMonth != null)
            {
                var payCalendarHeader =  await _repositoryPayCalendar.FirstOrDefaultAsync(x=> x.Year == currentPayMonth.Year);

                if (currentPayMonth != null)
                {
                    return payCalendarHeader.Details.FirstOrDefault(a =>
                        a.Month == currentPayMonth.Month && a.Year == currentPayMonth.Year);
                }
                
            }

            return null; 
        }

        public async Task<object> ListAll()
        {
            var data = await _repositoryPayCalendar.GetAllListAsync();
            return data.OrderByDescending(a => a.Year).ToList();
        }
        
        
        public async Task<object> ListAll(int pageNo, int pageSize, int year)
        {
            var predicate = PredicateBuilder.New<PayCalendar>();

            predicate.And(x => !x.IsDeleted);
            
            if (year > 0)
            {
                predicate.And(x => x.Year == year);
            }
            
            var totalCount =
                
                await _repositoryPayCalendar.CountAsync(predicate);
            
            var data = await (await _repositoryPayCalendar.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Year, a.Details, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
            await _repositoryPayCalendar.DeleteAsync(id);

        }

        //
        public async Task DeletePayCalendarDetails(PayCalendarDetailDto dto)
        {
            var payCalendarData = await _repositoryPayCalendar.GetAsync(dto.PayCalendarId);
            var calendarList = payCalendarData.Details;
            var calendarInfo = calendarList.FirstOrDefault(x => x.Id == dto.Id);

            calendarList.Remove(calendarInfo);

            payCalendarData.Details = calendarList;

            await _repositoryPayCalendar.UpdateAsync(payCalendarData);

        }
    
}