using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.FeesDescriptions;

public class FeesDescriptionManager: Abp.Domain.Services.DomainService, IFeesDescriptionManager
{
    private readonly IRepository<FeesDescription, Guid> _repositoryFeesDescription;
        private readonly IRepository<BillType, Guid> _repositoryBillType;

        public FeesDescriptionManager(IRepository<FeesDescription, Guid> repositoryFeesDescription, IRepository<BillType, Guid> repositoryBillType)
        {
            _repositoryFeesDescription = repositoryFeesDescription;
            _repositoryBillType = repositoryBillType;
        }

        public async Task Create(FeesDescription entity)
        {
            var datta = await _repositoryFeesDescription.FirstOrDefaultAsync(x => x.Description == entity.Description && x.BillTypeId == entity.BillTypeId);
            if (datta == null)
            {
                var billTypeInfo = await _repositoryBillType.FirstOrDefaultAsync(x=>x.Id == entity.BillTypeId);

                if (billTypeInfo != null)
                {
                    entity.BillTypeName = billTypeInfo.Name;
                }

                await _repositoryFeesDescription.InsertAsync(entity);

                return;
            }

            throw new UserFriendlyException(
                400,
                "Duplicate records are not allowed.",
                "A Fee Description with the same name already exists."
            );
            
        }

        public async Task<object> ListAll(int pageNo, int pageSize, string query)
        {
            var predicate = PredicateBuilder.New<FeesDescription>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.Description.Contains(query));
            }
            
            var totalCount =  await _repositoryFeesDescription.CountAsync(predicate);
            
            var data = await (await _repositoryFeesDescription.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Description, a.BillTypeName, a.ReceivableAccountName, a.BillTypeId, a.ReceivableAccountId }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        public async Task<object> ListAll(Guid billTypeId)
        {
            return await _repositoryFeesDescription.GetAllListAsync(a => a.BillTypeId == billTypeId);
        }
        public async Task Delete(Guid id)
        {
            await _repositoryFeesDescription.DeleteAsync(id);
        }

        public async Task Update(FeesDescription entity)
        {
            var billTypeInfo = await _repositoryBillType.FirstOrDefaultAsync(x=>x.Id == entity.BillTypeId);

            if (billTypeInfo != null)
            {
                entity.BillTypeName = billTypeInfo.Name;
            }

            await _repositoryFeesDescription.UpdateAsync(entity);
        }
}