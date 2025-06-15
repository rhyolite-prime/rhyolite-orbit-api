using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.BoaAllowances;

public class BoaAllowanceManager: Abp.Domain.Services.DomainService, IBoaAllowanceManager
{
        private readonly IRepository<BonusAndOnetimeAllowance, Guid> _repositoryBonusAndOnetimeAllowance;

        public BoaAllowanceManager(IRepository<BonusAndOnetimeAllowance, Guid> repositoryBonusAndOnetimeAllowance)
        {
            _repositoryBonusAndOnetimeAllowance = repositoryBonusAndOnetimeAllowance;
        }

        public async Task Create(BonusAndOnetimeAllowance entity)
        {
            var datta = await _repositoryBonusAndOnetimeAllowance.FirstOrDefaultAsync(x => x.EmployeeId == entity.EmployeeId && x.AllowanceTypeId == entity.AllowanceTypeId && x.Month == entity.Month && x.Year == entity.Year);

            if (datta == null)
            {
                await _repositoryBonusAndOnetimeAllowance.InsertAsync(entity);

            }
            else
            {

                datta.AllowanceTypeId = entity.AllowanceTypeId;
                datta.IsFixedAmount = entity.IsFixedAmount;
                datta.Amount = entity.Amount;
                datta.Percentage = entity.Percentage;
                datta.IsTaxable = entity.IsTaxable;
                datta.IsSsf = entity.IsSsf;
                datta.IsPf = entity.IsPf;
                datta.AllowanceDays = entity.AllowanceDays;
                await _repositoryBonusAndOnetimeAllowance.UpdateAsync(entity);

            }
        }

        public async Task Update(BonusAndOnetimeAllowance entity)
        {
            await _repositoryBonusAndOnetimeAllowance.UpdateAsync(entity);
        }

        public async Task<object> GetAsync(Guid id)
        {
            return await _repositoryBonusAndOnetimeAllowance.FirstOrDefaultAsync(id);
        }

        public async Task<object> ListAll(int pageNo, int pageSize, string query)
        {
            var predicate = PredicateBuilder.New<BonusAndOnetimeAllowance>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.AllowanceTypeName.Contains(query) || x.EmployeeName.Contains(query) || x.EmployeeIdentifier.Contains(query));
            }
            
            var totalCount =
                
                await _repositoryBonusAndOnetimeAllowance.CountAsync(predicate);
            
            var data = await (await _repositoryBonusAndOnetimeAllowance.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.IsTaxable, a.EmployeeId, a.EmployeeIdentifier, a.EmployeeName, a.IsPf, a.IsSsf, a.AllowanceTypeName, a.AllowanceTypeId, a.Amount, a.IsFixedAmount, a.Month, a.Year, a.AllowanceDays, a.Percentage,  }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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

        public async Task<object> ListAll(int month, int year)
        {
           return  await _repositoryBonusAndOnetimeAllowance.GetAllListAsync(x => x.Month == month && x.Year == year);

        }
        public async Task Delete(Guid id)
        {
            await _repositoryBonusAndOnetimeAllowance.DeleteAsync(id);

        }
}