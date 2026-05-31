using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.GeneralLedger;

namespace RhyoliteOrbit.DomainServices.GeneralLedger.CoaHierarchies;

public class AccountHeadManager: DomainService, IAccountHeadManager
{
        private readonly IRepository<AccountHead, Guid> _repositoryAccountHead;

        public AccountHeadManager(IRepository<AccountHead, Guid> repositoryAccountHead)
        {
            _repositoryAccountHead = repositoryAccountHead;
        }

        public async Task Create(AccountHead entity)
        {
            var datta = await _repositoryAccountHead.FirstOrDefaultAsync(x => x.Name == entity.Name);
            
            if (datta == null)
            {
                await _repositoryAccountHead.InsertAsync(entity);
                return;
            }

            throw new UserFriendlyException(
                400,
                "Duplicate records are not allowed.",
                "An Account Head with the same name already exists."
            );
            
        }

        public async Task Update(AccountHead entity)
        {
            await _repositoryAccountHead.UpdateAsync(entity);
        }

        public async Task<object> GetAsync(Guid id)
        {
           return await _repositoryAccountHead.FirstOrDefaultAsync(id);
        }

        public async Task<object> ListAll(int pageNo, int pageSize, string query)
        {
            var predicate = PredicateBuilder.New<AccountHead>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.Name.Contains(query));
            }
            
            var totalCount =
                
                await _repositoryAccountHead.CountAsync(predicate);
            
            var data = await (await _repositoryAccountHead.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.AccountHeadType }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
            await _repositoryAccountHead.DeleteAsync(id);

        }
}