using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeCategories;

public class EmployeeCategoryManager: Abp.Domain.Services.DomainService, IEmployeeCategoryManager
{
        private readonly IRepository<EmployeeCategory, Guid> _repositoryEmployeeCategory;

        public EmployeeCategoryManager(IRepository<EmployeeCategory, Guid> repositoryEmployeeCategory)
        {
            _repositoryEmployeeCategory = repositoryEmployeeCategory;
        }

        public async Task Create(EmployeeCategory entity)
        {
            var datta = await _repositoryEmployeeCategory.FirstOrDefaultAsync(x => x.Name.Trim().ToLower() == entity.Name.Trim().ToLower());

            if (datta == null)
            {
                await _repositoryEmployeeCategory.InsertAsync(entity);

                return ;
            }

            throw new UserFriendlyException(
                400,
                "Duplicate records are not allowed.",
                "An Employee Category with the same name already exists."
            );
            
        }

        public async Task Update(EmployeeCategory entity)
        {
            await _repositoryEmployeeCategory.UpdateAsync(entity);
        }

        public async Task<object> GetAsync(Guid id)
        {
            return await _repositoryEmployeeCategory.FirstOrDefaultAsync(id);
        }

        public async Task<EmployeeCategory> GetAsync(string name)
        {
            return await _repositoryEmployeeCategory.FirstOrDefaultAsync(x=> x.Name == name);
        }
        
        
        public async Task<object> ListAll(int pageNo, int pageSize, string query)
        {
            var predicate = PredicateBuilder.New<EmployeeCategory>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.Name.ToLower().Contains(query.ToLower()));
            }
            
            var totalCount =
                
                await _repositoryEmployeeCategory.CountAsync(predicate);
            
            var data = await (await _repositoryEmployeeCategory.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
            await _repositoryEmployeeCategory.DeleteAsync(id);

        }
}