using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.CustomerRanks;

public class CustomerRankManager: DomainService, ICustomerRankManager
    {

        private readonly IRepository<CustomerRank, Guid> _repositoryCustomerRank;

        public CustomerRankManager(IRepository<CustomerRank, Guid> repositoryCustomerRank)
        {
            _repositoryCustomerRank = repositoryCustomerRank;
        }
        
        public async Task<object> GetAll(int pageNo, int pageSize, string query)
        {
            
            var predicate = PredicateBuilder.New<CustomerRank>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.Name.ToLower().Contains(query.ToLower()));

            }
            
            var totalCount =
                
                await _repositoryCustomerRank.CountAsync(predicate);
            
            var data = await (await _repositoryCustomerRank.GetAllAsync()).Where(predicate).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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


        public async Task Create(CustomerRank entity)
        {
            var datta = await _repositoryCustomerRank.FirstOrDefaultAsync(x => x.Name == entity.Name);
            if (datta != null)
            {
                throw new UserFriendlyException(400, "Duplicate records are not allowed.");
            }
            
            await _repositoryCustomerRank.InsertAsync(entity);

        }


        public async Task Update(CustomerRank entity)
        {
            await _repositoryCustomerRank.UpdateAsync(entity);
        }

        public async Task Delete(Guid id)
        {
            await _repositoryCustomerRank.DeleteAsync(id);
        }

    }