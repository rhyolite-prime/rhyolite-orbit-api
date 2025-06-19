using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeDaysWorkedData;

public class EmployeeDaysWorkedManager: Abp.Domain.Services.DomainService, IEmployeeDaysWorkedManager
{
        private readonly IRepository<EmployeeDaysWorked, Guid> _repositoryEmployeeDaysWorked;
        private readonly IRepository<EmployeeBioData, Guid> _repositoryEmployeeBioData;
        private readonly IRepository<EmployeeSalaryData, Guid> _repositoryEmployeeSalaryData;
        private readonly IRepository<PayCalendar, Guid> _repositoryPayCalendar;
        private readonly IRepository<InitializePayMonth, Guid> _repositoryInitializePayMonth;

        public EmployeeDaysWorkedManager(IRepository<EmployeeDaysWorked, Guid> repositoryEmployeeDaysWorked, IRepository<EmployeeBioData, Guid> repositoryEmployeeBioData, IRepository<EmployeeSalaryData, Guid> repositoryEmployeeSalaryData, IRepository<PayCalendar, Guid> repositoryPayCalendar, IRepository<InitializePayMonth, Guid> repositoryInitializePayMonth)
        {
            _repositoryEmployeeDaysWorked = repositoryEmployeeDaysWorked;
            _repositoryEmployeeBioData = repositoryEmployeeBioData;
            _repositoryEmployeeSalaryData = repositoryEmployeeSalaryData;
            _repositoryPayCalendar = repositoryPayCalendar;
            _repositoryInitializePayMonth = repositoryInitializePayMonth;
        }

        public async Task Create(EmployeeDaysWorked entity)
        {

            var datta = await _repositoryEmployeeDaysWorked.FirstOrDefaultAsync(x => x.EmployeeId == entity.EmployeeId && x.Month == entity.Month && x.Year == entity.Year);
            if (datta == null)
            {
                var employeeInfo = await _repositoryEmployeeBioData.FirstOrDefaultAsync(entity.EmployeeId);

                if (employeeInfo != null)
                {
                    entity.EmployeeIdentifier = employeeInfo.EmployeeIdentifier;

                    entity.EmployeeName = string.IsNullOrEmpty(employeeInfo.OtherName) ? $"{employeeInfo.LastName} {employeeInfo.FirstName}" : $"{employeeInfo.LastName} {employeeInfo.FirstName} {employeeInfo.OtherName}";

                }
                await _repositoryEmployeeDaysWorked.InsertAsync(entity);

            }
            else
            {
                datta.Days = entity.Days;
                datta.Hours = entity.Hours;
                datta.Minutes = entity.Minutes;
                await _repositoryEmployeeDaysWorked.UpdateAsync(datta);

            }
        }

        public async Task Update(EmployeeDaysWorked entity)
        {
            await _repositoryEmployeeDaysWorked.UpdateAsync(entity);
        }

        public async Task<object> GetAsync(Guid id)
        {
            return await _repositoryEmployeeDaysWorked.FirstOrDefaultAsync(id);
        }

        public async Task<object> ListAll()
        {
            var data = await _repositoryEmployeeDaysWorked.GetAllListAsync();

            return data.OrderBy(a => a.EmployeeName);

        }

        public async Task<object> ListAll(int month, int year, string salaryType)
        {
            var employeeDaysWorked  = await _repositoryEmployeeDaysWorked.GetAllListAsync(a => a.Month == month && a.Year == year);
            var employees = await _repositoryEmployeeBioData.GetAllListAsync();
            var employeeSalaryData = await _repositoryEmployeeSalaryData.GetAllListAsync(a => a.SalaryType == salaryType);

            
            var payYear = await _repositoryPayCalendar.FirstOrDefaultAsync(x => x.Year == year);

            int defaultDays = 0;
            
            var payMonth = payYear?.PayCalendarDetails?.FirstOrDefault(x => x.Year == year && x.Month == month);
            if (payMonth is { Days: > 0 })
            {
                defaultDays = payMonth.Days;
            }
            
            
            var query = from u1 in employees
                        join u2 in employeeSalaryData on u1.Id equals u2.EmployeeId
                        join u3 in employeeDaysWorked on u1.Id equals u3.EmployeeId into aa
                        from f in aa.DefaultIfEmpty()
                        select new
                        {
                            u1.EmployeeIdentifier,
                            u1.LastName,
                            u1.FirstName,
                            u1.OtherName,
                            EmployeeName = string.IsNullOrEmpty(u1.OtherName) ? $"{u1.LastName} {u1.FirstName}" : $"{u1.LastName} {u1.FirstName} {u1.OtherName}",
                            EmployeeId = u1.Id,
                            Days = f?.Days ?? defaultDays,
                            Hours = f?.Hours ?? 0,
                            Minutes = f?.Minutes ?? 0,
                            u1.TenantId
                        };

            return query.OrderBy(a => a.EmployeeName).ToList();

        }
        public async Task Delete(Guid id)
        {
            await _repositoryEmployeeDaysWorked.DeleteAsync(id);

        }
}