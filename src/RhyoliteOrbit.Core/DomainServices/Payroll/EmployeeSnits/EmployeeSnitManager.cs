using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeSnits;

public class EmployeeSnitManager: Abp.Domain.Services.DomainService, IEmployeeSnitManager
{
        private readonly IRepository<EmployeeSnit, Guid> _repositoryEmployeeSnit;

        public EmployeeSnitManager(IRepository<EmployeeSnit, Guid> repositoryEmployeeSnit)
        {
            _repositoryEmployeeSnit = repositoryEmployeeSnit;
        }

        public async Task Create(EmployeeSnit entity)
        {
            var datta = await _repositoryEmployeeSnit.FirstOrDefaultAsync(x => x.EmployeeId == entity.EmployeeId);

            if (datta == null)
            {
                await _repositoryEmployeeSnit.InsertAsync(entity);

            }
            else
            {
                datta.SecondProvidentFundEmployeeContribution = entity.SecondProvidentFundEmployeeContribution;
                datta.SecondProvidentFundEmployerContribution = entity.SecondProvidentFundEmployerContribution;
                datta.ProvidentFundEmployeeContribution = entity.ProvidentFundEmployeeContribution;
                datta.ProvidentFundEmployerContribution = entity.ProvidentFundEmployerContribution;
                datta.SuperAnnuationEmployeeContribution = entity.SuperAnnuationEmployeeContribution;
                datta.SuperAnnuationEmployerContribution = entity.SuperAnnuationEmployerContribution;
                datta.SocialSecurityFundEmployeeContribution = entity.SocialSecurityFundEmployeeContribution;
                datta.SocialSecurityFundEmployerContribution = entity.SocialSecurityFundEmployerContribution;

                await _repositoryEmployeeSnit.UpdateAsync(datta);

            }
        }

        public async Task Update(EmployeeSnit entity)
        {
            await _repositoryEmployeeSnit.UpdateAsync(entity);
        }

        public async Task<object> GetAsync(Guid employeeId)
        {
            return await _repositoryEmployeeSnit.FirstOrDefaultAsync(x=>x.EmployeeId == employeeId);
        }

         
        public async Task<object> ListAll(int pageNo, int pageSize, string query)
        {
            var predicate = PredicateBuilder.New<EmployeeSnit>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.SocialSecurityNo.Contains(query) || x.EmployeeName.Contains(query) || x.EmployeeIdentifier.Contains(query));
            }
            
            var totalCount =
                
                await _repositoryEmployeeSnit.CountAsync(predicate);
            
            var data = await (await _repositoryEmployeeSnit.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.EmployeeId, a.EmployeeIdentifier, a.EmployeeName, a.SocialSecurityNo, a.SocialSecurityFundEmployerContribution, a.SocialSecurityFundEmployeeContribution, a.SuperAnnuationEmployerContribution, a.SuperAnnuationEmployeeContribution, a.ProvidentFundEmployeeContribution, a.ProvidentFundEmployerContribution, a.SecondProvidentFundEmployeeContribution, a.SecondProvidentFundEmployerContribution, a.PaySocialSecurityFund, a.PayProvidentFund, a.PaySecondProvidentFund   }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
            await _repositoryEmployeeSnit.DeleteAsync(id);

        }
    
}