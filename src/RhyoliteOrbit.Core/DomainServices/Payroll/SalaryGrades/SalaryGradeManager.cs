using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.DomainServices.Payroll.SalaryGrades.Dto;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.SalaryGrades;

public class SalaryGradeManager: Abp.Domain.Services.DomainService, ISalaryGradeManager
{
        private readonly IRepository<SalaryGrade, Guid> _repositorySalaryGrade;
         
        public SalaryGradeManager(IRepository<SalaryGrade, Guid> repositorySalaryGrade)
        {
            _repositorySalaryGrade = repositorySalaryGrade;
            
        }

        public async Task<object> Create(SalaryGrade entity)
        {
            var datta = await _repositorySalaryGrade.FirstOrDefaultAsync(x => x.Name.Trim().ToLower() == entity.Name.Trim().ToLower());

            if (datta == null)
            {
                await _repositorySalaryGrade.InsertAsync(entity);

                return new
                {
                    code = 200,
                    message = "successful"
                };
            }

            throw new UserFriendlyException(
                400,
                "Duplicate Salary Grade record.",
                "An Salary Grade with the same name already exists."
            );
            
        }

        public async Task<object> CreateSalaryNotch(SalaryNotchDto input)
        {
            var salaryGradeData = await _repositorySalaryGrade.GetAsync(input.SalaryGradeId);
            var notchList = salaryGradeData.SalaryNotches;
            var notchInfo = notchList.FirstOrDefault(x => x.Id == input.Id);

            if (notchInfo == null)
            {
                salaryGradeData.SalaryNotches.Add(new SalaryNotch
                {
                    Id = Guid.NewGuid(),
                    SalaryGradeId = input.SalaryGradeId,
                    SalaryGradeName = salaryGradeData.Name,
                    Notch = input.Notch,
                    Salary = input.Salary,

                });

                await _repositorySalaryGrade.UpdateAsync(salaryGradeData);

                return new
                {
                    code = 200,
                    message = "successful"
                };
            }

            throw new UserFriendlyException(
                400,
                "Duplicate Salary Notch record.",
                "An Salary Notch with the same SalaryGradeId and Notch already exists."
            );
            
             
        }
       
        public async Task Update(SalaryGrade entity)
        {
            await _repositorySalaryGrade.UpdateAsync(entity);
        }

        public async Task UpdateSalaryNotch(SalaryNotchDto input)
        {
            var salaryGradeData = await _repositorySalaryGrade.GetAsync(input.SalaryGradeId);
            var notchList = salaryGradeData.SalaryNotches;
            var notchInfo = notchList.FirstOrDefault(x => x.Id == input.Id);

            notchList.Remove(notchInfo);

            if (notchInfo != null)
            {
                notchInfo.SalaryGradeName = salaryGradeData.Name;
                notchInfo.Notch = input.Notch;
                notchInfo.Salary = input.Salary;
                notchInfo.NumericalOrder = input.NumericalOrder;

                notchList.Add(notchInfo);
            }

            salaryGradeData.SalaryNotches = notchList;

            await _repositorySalaryGrade.UpdateAsync(salaryGradeData);
        }
        public async Task<object> GetAsync(Guid id)
        {
            return await _repositorySalaryGrade.FirstOrDefaultAsync(id);
        }

        public async Task<SalaryGrade> GetAsync(string name)
        {
            return await _repositorySalaryGrade.FirstOrDefaultAsync(x=> x.Name == name);
        }

        public async Task<object> ListAll()
        {
            var data = await _repositorySalaryGrade.GetAllListAsync();

            return data.OrderBy(a => a.Name).ToList();
        }
        
        
        public async Task<object> ListAll(int pageNo, int pageSize, string query)
        {
            var predicate = PredicateBuilder.New<SalaryGrade>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.Name.Contains(query));
            }
            
            var totalCount =
                
                await _repositorySalaryGrade.CountAsync(predicate);
            
            var data = await (await _repositorySalaryGrade.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.SalaryNotches, a.NumericalOrder,  }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
            await _repositorySalaryGrade.DeleteAsync(id);
        }

        public async Task DeleteSalaryNotch(SalaryNotchDto input)
        {
            var salaryGradeData = await _repositorySalaryGrade.GetAsync(input.SalaryGradeId);
            var notchList = salaryGradeData.SalaryNotches;
            var notchInfo = notchList.FirstOrDefault(x => x.Id == input.Id);

            notchList.Remove(notchInfo);

            salaryGradeData.SalaryNotches = notchList;

            await _repositorySalaryGrade.UpdateAsync(salaryGradeData);

        }
}