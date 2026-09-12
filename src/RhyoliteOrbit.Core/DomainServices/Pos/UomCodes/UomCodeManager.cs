using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.UomCodes;

public class UomCodeManager: DomainService, IUomCodeManager
{
    private readonly IRepository<UomCode, Guid> _repositoryUomCode;

        public UomCodeManager(IRepository<UomCode, Guid> repositoryUomCode)
        {
            _repositoryUomCode = repositoryUomCode;
        }

        public async Task<object> GetAll(int pageNo, int pageSize, string query)
        {
            
            var predicate = PredicateBuilder.New<UomCode>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.Name.ToLower().Contains(query.ToLower()));

            }
            
            var totalCount =
                
                await _repositoryUomCode.CountAsync(predicate);
            
            var data = await (await _repositoryUomCode.GetAllAsync()).Where(predicate).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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


        public async Task Create(UomCode entity)
        {
            var existingUomCode = await _repositoryUomCode.FirstOrDefaultAsync(x => x.Name == entity.Name);
            if (existingUomCode != null)
            {
                throw new UserFriendlyException(400, "Duplicate records are not allowed.");
            }

            await _repositoryUomCode.InsertAsync(entity);
        }


        public async Task Update(UomCode entity)
        {
            await _repositoryUomCode.UpdateAsync(entity);
        }

        public async Task Delete(Guid id)
        {
            await _repositoryUomCode.DeleteAsync(id);
        }
}