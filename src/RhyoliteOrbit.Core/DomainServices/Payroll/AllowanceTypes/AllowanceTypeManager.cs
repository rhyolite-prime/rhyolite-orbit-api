using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.DomainServices.Payroll.AllowanceTypes.Dto;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.AllowanceTypes;

public class AllowanceTypeManager: Abp.Domain.Services.DomainService, IAllowanceTypeManager
{
        private readonly IRepository<AllowanceType, Guid> _repositoryAllowanceType;
        private readonly IRepository<EmployeeSalaryData, Guid> _repositoryEmployeeSalaryData;

        public AllowanceTypeManager(IRepository<AllowanceType, Guid> repositoryAllowanceType, IRepository<EmployeeSalaryData, Guid> repositoryEmployeeSalaryData)
        {
            _repositoryAllowanceType = repositoryAllowanceType;
            _repositoryEmployeeSalaryData = repositoryEmployeeSalaryData;
        }

        public async Task<object> Create(AllowanceType entity)
        {
            var datta = await _repositoryAllowanceType.FirstOrDefaultAsync(x => x.Name.Trim().ToLower() == entity.Name.Trim().ToLower());

            if (datta == null)
            {
                entity.Name = entity.Name.Trim();
                await _repositoryAllowanceType.InsertAsync(entity);

                return new
                {
                    code = 200,
                    message = "successful"
                };
            }
            
            throw new UserFriendlyException(
                400,
                "Allowance Type",
                "Duplicate records are not allowed."
            );

        }

        public async Task<object> CreateAllowanceRate(AllowanceRateDto input)
        {
            var allowanceTypeData = await _repositoryAllowanceType.GetAsync(input.AllowanceTypeId);
            var ratesList = allowanceTypeData.AllowanceRates;
            var rateInfo = ratesList.FirstOrDefault(x => x.Id == input.Id);

            if (rateInfo == null)
            {
                var existingAllowanceRate =
                    allowanceTypeData.AllowanceRates.FirstOrDefault(x => x.AllowanceTypeId == input.AllowanceTypeId && x.EmployeeCategoryId == input.EmployeeCategoryId);

                if (existingAllowanceRate != null)
                {
                    return new
                    {
                        code = 400,
                        message = "Duplicate records are not allowed."
                    };
                }
                
                allowanceTypeData.AllowanceRates.Add(new AllowanceRate
                {
                    Id = input.Id,
                    AllowanceTypeName = allowanceTypeData.Name,
                    AllowanceTypeId = input.AllowanceTypeId, 
                    Amount = input.Amount, 
                    ApplyBackPay = input.ApplyBackPay, 
                    EmployeeCategoryId = input.EmployeeCategoryId, 
                    EmployeeCategoryName = input.EmployeeCategoryName, 
                    FixedAmount = input.FixedAmount, 
                    MaximumAmount = input.MaximumAmount, 
                    PercentageBasic = input.PercentageBasic, 
                    Prorate = input.Prorate,
                      
                });

                await _repositoryAllowanceType.UpdateAsync(allowanceTypeData);

                return new
                {
                    code = 200,
                    message = "successful"
                };
            }

            return new
            {
                code = 400,
                message = "Duplicate records are not allowed."
            };
        }

        public async Task Update(AllowanceType entity)
        {
            entity.Name = entity.Name.Trim();
            await _repositoryAllowanceType.UpdateAsync(entity);
        }

        public async Task UpdateAllowanceRate(AllowanceRateDto input)
        {
            var allowanceTypeData = await _repositoryAllowanceType.GetAsync(input.AllowanceTypeId);
            var ratesList = allowanceTypeData.AllowanceRates;
            var rateInfo = ratesList.FirstOrDefault(x => x.Id == input.Id);
            ratesList.Remove(rateInfo);

            if (rateInfo != null)
            {
                rateInfo.AllowanceTypeName = allowanceTypeData.Name;
                rateInfo.AllowanceTypeId = input.AllowanceTypeId;
                rateInfo.Amount = input.Amount;
                rateInfo.ApplyBackPay = input.ApplyBackPay;
                rateInfo.EmployeeCategoryId = input.EmployeeCategoryId;
                rateInfo.EmployeeCategoryName = input.EmployeeCategoryName;
                rateInfo.FixedAmount = input.FixedAmount;
                rateInfo.MaximumAmount = input.MaximumAmount;
                rateInfo.PercentageBasic = input.PercentageBasic;
                rateInfo.Prorate = input.Prorate;

                ratesList.Add(rateInfo);
            }

            allowanceTypeData.AllowanceRates = ratesList;

            await _repositoryAllowanceType.UpdateAsync(allowanceTypeData);
        }

        public async Task<AllowanceType> GetAsync(Guid id)
        {
            return await _repositoryAllowanceType.FirstOrDefaultAsync(id);
        }

        public async Task<AllowanceType> GetAsync(string name)
        {
            return await _repositoryAllowanceType.FirstOrDefaultAsync(x=> x.Name.ToLower() == name.Trim().ToLower());
        }

        public async Task<object> ListAll(int pageNo, int pageSize, string query)
        {
            var predicate = PredicateBuilder.New<AllowanceType>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.Name.Contains(query));
            }
            
            var totalCount =
                
                await _repositoryAllowanceType.CountAsync(predicate);
            
            var data = await (await _repositoryAllowanceType.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.ExpenseAccountId, a.ExpenseAccountName, a.AllowanceRates, a.IsTaxable  }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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

        public async Task<object> ListAll(Guid categoryId)
        {

            List<AllowanceRate> allowanceRates = new List<AllowanceRate>();

            List<AllowanceType> allowanceTypes = new List<AllowanceType>();

            var data = await _repositoryAllowanceType.GetAllListAsync();

            data.ForEach((a) =>
            {
                allowanceRates.AddRange(a.AllowanceRates);
            });

            var filteredAllowanceRates = allowanceRates.Where(o => categoryId != Guid.Empty && o.EmployeeCategoryId == categoryId).ToList();

            filteredAllowanceRates.ForEach((a) =>
            {
                var allowanceType = data.FirstOrDefault(x => x.Id == a.AllowanceTypeId);
                allowanceTypes.Add(allowanceType);

            });

            return allowanceTypes.OrderBy(a=>a.Name);

        }

        public async Task<object> ListAll(Guid allowanceTypeId, Guid employeeId, Guid categoryId)
        {

            var salaryInfo = await _repositoryEmployeeSalaryData.FirstOrDefaultAsync(a => a.EmployeeId == employeeId);

            List<AllowanceRate> allowanceRates = new List<AllowanceRate>();

            var data = await _repositoryAllowanceType.GetAllListAsync();

            data.ForEach((a) =>
            {
                allowanceRates.AddRange(a.AllowanceRates);
            });

            var allowanceRate = allowanceRates.FirstOrDefault(o => categoryId != Guid.Empty && o.EmployeeCategoryId == categoryId && o.AllowanceTypeId == allowanceTypeId);

            var associatedAllowanceType = data.FirstOrDefault(b => b.Id == allowanceTypeId);

            allowanceRate.Taxable = associatedAllowanceType.IsTaxable;

            if (allowanceRate != null)
            {
                if (allowanceRate.FixedAmount)
                {
                    allowanceRate.Amount = allowanceRate.Amount;
                    if (allowanceRate.MaximumAmount != 0 && allowanceRate.Amount > allowanceRate.MaximumAmount)
                    {
                        allowanceRate.Amount = allowanceRate.MaximumAmount;
                    }
                }
                else if (!allowanceRate.FixedAmount)
                {
                    allowanceRate.Amount = (allowanceRate.PercentageBasic * salaryInfo.MonthlySalary) / 100;
                    if (allowanceRate.MaximumAmount != 0 && allowanceRate.Amount > allowanceRate.MaximumAmount)
                    {
                        allowanceRate.Amount = allowanceRate.MaximumAmount;
                    }
                }
            }
             


            return allowanceRate;


        }
        public async Task Delete(Guid id)
        {
            await _repositoryAllowanceType.DeleteAsync(id);
        }

        public async Task DeleteAllowanceRate(AllowanceRateDto input)
        {
            var allowanceTypeData = await _repositoryAllowanceType.GetAsync(input.AllowanceTypeId);
            var ratesList = allowanceTypeData.AllowanceRates;
            var rateInfo = ratesList.FirstOrDefault(x => x.Id == input.Id);
            
            ratesList.Remove(rateInfo);

            allowanceTypeData.AllowanceRates = ratesList;

            await _repositoryAllowanceType.UpdateAsync(allowanceTypeData);

        }
}