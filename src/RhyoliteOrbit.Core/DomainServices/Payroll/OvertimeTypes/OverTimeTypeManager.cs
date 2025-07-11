using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.DomainServices.Payroll.OvertimeTypes.Dto;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.OvertimeTypes;

public class OverTimeTypeManager: Abp.Domain.Services.DomainService, IOverTimeTypeManager
{
    private readonly IRepository<OvertimeType, Guid> _repositoryOverTimeType;

        public OverTimeTypeManager(IRepository<OvertimeType, Guid> repositoryOverTimeType)
        {
            _repositoryOverTimeType = repositoryOverTimeType;
        }

        public async Task<object> Create(OvertimeType entity)
        {
            var datta = await _repositoryOverTimeType.FirstOrDefaultAsync(x => x.Name.Trim().ToLower() == entity.Name.Trim().ToLower());

            if (datta == null)
            {
                await _repositoryOverTimeType.InsertAsync(entity);

                return new
                {
                    code = 200,
                    message = "successful"
                };
            }

            throw new UserFriendlyException(
                400,
                "Duplicate records are not allowed.",
                "An Overtime Type with the same name already exists."
            );
            
        }

        
        public async Task CreateOvertimeRate(OvertimeRateDto dto)
        {
            var overtimeTypeData = await _repositoryOverTimeType.GetAsync(dto.OvertimeTypeId);

            if (overtimeTypeData.Rates == null)
            {
                overtimeTypeData.Rates = new List<OvertimeRate>();
            }

            var rateInfo = overtimeTypeData.Rates.FirstOrDefault(x => x.Id == dto.Id);

            if (rateInfo == null)
            {
                overtimeTypeData.Rates.Add(new OvertimeRate
                {
                    Id = dto.Id,
                    Amount = dto.Amount,
                    OvertimeTypeId = dto.OvertimeTypeId,
                    EmployeeCategoryId = dto.EmployeeCategoryId,
                    EmployeeCategoryName = dto.EmployeeCategoryName,
                    FixedAmount = dto.FixedAmount,
                    MaximumAmount = dto.MaximumAmount,
                    PercentageBasic = dto.PercentageBasic,
                    IsFactor = dto.IsFactor,
                    AnnualHours = dto.AnnualHours,
                    PercentageLimitOfBasic = dto.PercentageLimitOfBasic,
                    Prorate = dto.Prorate,
                });

                await _repositoryOverTimeType.UpdateAsync(overtimeTypeData);
                return;
            }

            throw new UserFriendlyException(
                400,
                "Duplicate records are not allowed.",
                "An Overtime Rate with the same ID already exists."
            );
        }

        public async Task Update(OvertimeType entity)
        {
            var overtimeTypeData = await _repositoryOverTimeType.GetAsync(entity.Id);
            if (overtimeTypeData == null)
            {
                throw new UserFriendlyException(
                    400,
                    "Overtime Type Error",
                    "An error occurred while retrieving the overtime type. Provide a valid overtime type id."
                );
            }
            
            overtimeTypeData.Name = entity.Name.Trim();
            overtimeTypeData.IsTaxable = entity.IsTaxable;
            
            await _repositoryOverTimeType.UpdateAsync(overtimeTypeData);
        }

        public async Task UpdateOvertimeRate(OvertimeRateDto input)
        {
            var overtimeTypeData = await _repositoryOverTimeType.GetAsync(input.OvertimeTypeId);
            var ratesList = overtimeTypeData.Rates;
            var rateInfo = ratesList.FirstOrDefault(x => x.Id == input.Id);
            
            ratesList.Remove(rateInfo);

            if (rateInfo != null)
            {
                rateInfo.Amount = input.Amount;
                rateInfo.OvertimeTypeId = input.OvertimeTypeId;
                rateInfo.EmployeeCategoryId = input.EmployeeCategoryId;
                rateInfo.EmployeeCategoryName = input.EmployeeCategoryName;
                rateInfo.FixedAmount = input.FixedAmount;
                rateInfo.MaximumAmount = input.MaximumAmount;
                rateInfo.PercentageBasic = input.PercentageBasic;
                rateInfo.IsFactor = input.IsFactor;
                rateInfo.Prorate = input.Prorate;
                rateInfo.Factor = input.Factor;
                rateInfo.AnnualHours = input.AnnualHours;
                rateInfo.PercentageLimitOfBasic = input.PercentageLimitOfBasic;
                
                ratesList.Add(rateInfo);

                overtimeTypeData.Rates = ratesList;

                await _repositoryOverTimeType.UpdateAsync(overtimeTypeData);
                
            }
            
        }


        public async Task<OvertimeType> GetAsync(Guid id)
        {
            return await _repositoryOverTimeType.FirstOrDefaultAsync(id);
        }

        public async Task<object> ListAll(int pageNo, int pageSize, string query)
        {
            var predicate = PredicateBuilder.New<OvertimeType>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.Name.ToLower().Contains(query.ToLower()));
            }
            
            var totalCount =
                
                await _repositoryOverTimeType.CountAsync(predicate);
            
            var data = await (await _repositoryOverTimeType.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.Rates, a.IsTaxable  }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

            var totalPages = totalCount > 0 ? Convert.ToInt32(Math.Ceiling(totalCount / (decimal)pageSize)) : 0;

            var paginatedResult = new
            {
                pageNo,
                pageSize,
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
            await _repositoryOverTimeType.DeleteAsync(id);

        }

        public async Task DeleteOvertimeRate(Guid overtimeTypeId, Guid id)
        {
            var overtimeTypeData = await _repositoryOverTimeType.GetAsync(overtimeTypeId);
            var ratesList = overtimeTypeData.Rates;
            var rateInfo = ratesList.FirstOrDefault(x => x.Id == id);
            ratesList.Remove(rateInfo);

            overtimeTypeData.Rates = ratesList;

            await _repositoryOverTimeType.UpdateAsync(overtimeTypeData);

        }
}