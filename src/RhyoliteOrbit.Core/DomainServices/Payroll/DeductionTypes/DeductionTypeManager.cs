using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.DomainServices.Payroll.DeductionTypes.Dto;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.DeductionTypes;

public class DeductionTypeManager: Abp.Domain.Services.DomainService, IDeductionTypeManager
{
    private readonly IRepository<DeductionType, Guid> _repositoryDeductionType;
        private readonly IRepository<EmployeeSalaryData, Guid> _repositoryEmployeeSalaryData;
        private readonly IRepository<EmployeeCategory, Guid> _repositoryEmployeeCategory;
         
        public DeductionTypeManager(IRepository<DeductionType, Guid> repositoryDeductionType, IRepository<EmployeeSalaryData, Guid> repositoryEmployeeSalaryData, IRepository<EmployeeCategory, Guid> repositoryEmployeeCategory)
        {
            _repositoryDeductionType = repositoryDeductionType;
            _repositoryEmployeeSalaryData = repositoryEmployeeSalaryData;
            _repositoryEmployeeCategory = repositoryEmployeeCategory;
        }

        public async Task<object> Create(DeductionType entity)
        {
            var datta = await _repositoryDeductionType.FirstOrDefaultAsync(x => x.Name.Trim().ToLower() == entity.Name.Trim().ToLower());

            if (datta == null)
            {
                await _repositoryDeductionType.InsertAsync(entity);

                return new
                {
                    code = 200,
                    message = "successful"
                };
            }

            throw new UserFriendlyException(400, "Deduction Types","Duplicate records are not allowed.");
             
        }

        public async Task<object> CreateDeductionRate(DeductionRateDto dto)
        {
            var deductionTypeData = await _repositoryDeductionType.GetAsync(dto.DeductionTypeId);
            var ratesList = deductionTypeData.Rates;
            var rateInfo = ratesList.FirstOrDefault(x => x.Id == dto.Id);

            if (rateInfo == null)
            {
                
                var existingDeductionRate =
                    deductionTypeData.Rates.FirstOrDefault(x => x.DeductionTypeId == dto.DeductionTypeId && x.EmployeeCategoryId == dto.EmployeeCategoryId);

                if (existingDeductionRate != null)
                {
                    return new
                    {
                        code = 400,
                        message = "Duplicate records are not allowed."
                    };
                }
                
                //get category name...
                
                var employeeCategoryInfo = await _repositoryEmployeeCategory.FirstOrDefaultAsync(dto.EmployeeCategoryId);
                
                deductionTypeData.Rates.Add(new DeductionRate
                {
                    Id = Guid.NewGuid(),
                    Amount = dto.Amount,
                    DeductionTypeId = dto.DeductionTypeId,
                    EmployeeCategoryId = dto.EmployeeCategoryId,
                    EmployeeCategoryName = employeeCategoryInfo != null ? employeeCategoryInfo.Name : string.Empty,
                    DeductionTypeName = deductionTypeData.Name,
                    FixedAmount = dto.FixedAmount,
                    MaximumAmount = dto.MaximumAmount,
                    PercentageBasic = dto.PercentageBasic,
                    Prorate = dto.Prorate,
                });

                await _repositoryDeductionType.UpdateAsync(deductionTypeData);

                return new
                {
                    code = 200,
                    message = "successful"
                };
            }
            throw new UserFriendlyException(400, "Deduction Types","Duplicate records are not allowed.");
            
        }

        public async Task Update(DeductionType entity)
        {
            await _repositoryDeductionType.UpdateAsync(entity);
        }


        public async Task UpdateDeductionRate(DeductionRateDto dto)
        {
            var deductionTypeData = await _repositoryDeductionType.GetAsync(dto.DeductionTypeId);
            var ratesList = deductionTypeData.Rates;
            var rateInfo = ratesList.FirstOrDefault(x => x.Id == dto.Id);

            ratesList.Remove(rateInfo);

            if (rateInfo != null)
            {
                //get category name...
                
                var employeeCategoryInfo = await _repositoryEmployeeCategory.FirstOrDefaultAsync(dto.EmployeeCategoryId);

                rateInfo.Amount = dto.Amount;
                rateInfo.EmployeeCategoryId = dto.EmployeeCategoryId;
                rateInfo.EmployeeCategoryName = employeeCategoryInfo != null ? employeeCategoryInfo.Name : string.Empty;
                rateInfo.FixedAmount = dto.FixedAmount;
                rateInfo.MaximumAmount = dto.MaximumAmount;
                rateInfo.PercentageBasic = dto.PercentageBasic;
                rateInfo.DeductionTypeName = deductionTypeData.Name;
                rateInfo.Prorate = dto.Prorate;
                ratesList.Add(rateInfo);
                
                deductionTypeData.Rates = ratesList;

                await _repositoryDeductionType.UpdateAsync(deductionTypeData);
            }

           
        }


        public async Task<object> GetAsync(Guid id)
        {
            return await _repositoryDeductionType.FirstOrDefaultAsync(id);
        }

        public async Task<DeductionType> GetAsync(string name)
        {
            return await _repositoryDeductionType.FirstOrDefaultAsync(x => x.Name.ToLower() == name.Trim().ToLower());
        }
         
        
        public async Task<object> ListAll(int pageNo, int pageSize, string query)
        {
            var predicate = PredicateBuilder.New<DeductionType>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.Name.Contains(query) || x.LiabilityAccountName.Contains(query));
            }
            
            var totalCount =
                
                await _repositoryDeductionType.CountAsync(predicate);
            
            var data = await (await _repositoryDeductionType.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.LiabilityAccountName, a.LiabilityAccountId, a.Name, a.Rates }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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

        public async Task<object> ListAll(Guid deductionTypeId, Guid employeeId, Guid categoryId)
        {

            List<DeductionRate> deductionRates = new List<DeductionRate>();

            var deductionTypes = await _repositoryDeductionType.GetAllListAsync();

            deductionTypes.ForEach((a) =>
            {
                deductionRates.AddRange(a.Rates);
            });

            var deductionRate = deductionRates.FirstOrDefault(o => categoryId != Guid.Empty && o.EmployeeCategoryId == categoryId && o.DeductionTypeId == deductionTypeId);

            var salaryInfo = await _repositoryEmployeeSalaryData.FirstOrDefaultAsync(a => a.EmployeeId == employeeId);

            if (deductionRate != null)
            {
                if (deductionRate.FixedAmount)
                {
                    deductionRate.Amount = deductionRate.Amount;
                    if (deductionRate.MaximumAmount != 0 && deductionRate.Amount > deductionRate.MaximumAmount)
                    {
                        deductionRate.Amount = deductionRate.MaximumAmount;
                    }
                }
                else if (!deductionRate.FixedAmount)
                {
                    deductionRate.Amount = (deductionRate.PercentageBasic * salaryInfo.MonthlySalary) / 100;
                    if (deductionRate.MaximumAmount != 0 && deductionRate.Amount > deductionRate.MaximumAmount)
                    {
                        deductionRate.Amount = deductionRate.MaximumAmount;
                    }
                }
            }

            return deductionRate;
        }

        public async Task<object> ListAll(Guid categoryId)
        {
            List<DeductionRate> deductionRates = new List<DeductionRate>();

            List<DeductionType> deductionTypes = new List<DeductionType>();

            var data = await _repositoryDeductionType.GetAllListAsync();

            data.ForEach((a) =>
            {
                deductionRates.AddRange(a.Rates);
            });

            var filteredDeductionRates = deductionRates.Where(o => categoryId != Guid.Empty && o.EmployeeCategoryId == categoryId).ToList();

            filteredDeductionRates.ForEach((a) =>
            {
                var deductionType = data.FirstOrDefault(x=> x.Id == a.DeductionTypeId);
                deductionTypes.Add(deductionType);

            });

            return deductionTypes.OrderBy(a=>a.Name).ToList();

        }

        public async Task Delete(Guid id)
        {
            await _repositoryDeductionType.DeleteAsync(id);
        }

        public async Task DeleteDeductionRate(DeductionRateDto dto)
        {
            var deductionTypeData = await _repositoryDeductionType.GetAsync(dto.DeductionTypeId);
            var ratesList = deductionTypeData.Rates;
            var rateInfo = ratesList.FirstOrDefault(x => x.Id == dto.Id);
            ratesList.Remove(rateInfo);

            deductionTypeData.Rates = ratesList;

            await _repositoryDeductionType.UpdateAsync(deductionTypeData);

        }
    
    
}