using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.LoanTypes;

public class LoanTypeManager: Abp.Domain.Services.DomainService, ILoanTypeManager
{
        private readonly IRepository<LoanType, Guid> _repositoryLoanType;

        public LoanTypeManager(IRepository<LoanType, Guid> repositoryLoanType)
        {
            _repositoryLoanType = repositoryLoanType;
        }

        public async Task<object> Create(LoanType entity)
        {
            var datta = await _repositoryLoanType.FirstOrDefaultAsync(x => x.Name.Trim().ToLower() == entity.Name.Trim().ToLower());

            if (datta == null)
            {
                await _repositoryLoanType.InsertAsync(entity);

                return new
                {
                    code = 200,
                    message = "successful"
                };
            }

            throw new UserFriendlyException(
                400,
                "Duplicate records are not allowed.",
                "A Loan Type with the same name already exists."
            );
             
        }

        public async Task Update(LoanType entity)
        {
            await _repositoryLoanType.UpdateAsync(entity);
        }

        public async Task<LoanType> GetAsync(string name)
        {
            return await _repositoryLoanType.FirstOrDefaultAsync(x=> x.Name.ToLower() == name.ToLower().Trim());
        }

        public async Task<object> GetAsync(Guid id)
        {
            return await _repositoryLoanType.FirstOrDefaultAsync(id);
        }

        public async Task<object> ListAll(int pageNo, int pageSize, string query)
        {
            var predicate = PredicateBuilder.New<LoanType>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.Name.Contains(query));
            }
            
            var totalCount =
                
                await _repositoryLoanType.CountAsync(predicate);
            
            
            var data = await (await _repositoryLoanType.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name,}).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
            await _repositoryLoanType.DeleteAsync(id);
        }
    
}