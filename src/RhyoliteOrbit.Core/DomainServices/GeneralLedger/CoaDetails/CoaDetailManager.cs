using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.GeneralLedger;

namespace RhyoliteOrbit.DomainServices.GeneralLedger.CoaDetails;

public class CoaDetailManager: DomainService, ICoaDetailManager
{
        private readonly IRepository<CoaDetail, Guid> _repositoryCoaDetail;
        private readonly IRepository<CoaControl, Guid> _repositoryCoaControl;

        public CoaDetailManager(IRepository<CoaDetail, Guid> repositoryCoaDetail, IRepository<CoaControl, Guid> repositoryCoaControl)
        {
            _repositoryCoaDetail = repositoryCoaDetail;
            _repositoryCoaControl = repositoryCoaControl;
        }

        
        public async Task Create(CoaDetail entity)
        {
            var coaControl = await _repositoryCoaControl.FirstOrDefaultAsync(x => x.Id == entity.AccountHeaderId);
        
            if (entity.AccountNo >= coaControl.MinAccount && entity.AccountNo <= coaControl.MaxAccount)
            {
                // Check for duplicate AccountNo
                var exists = await _repositoryCoaDetail.FirstOrDefaultAsync(x => x.AccountNo == entity.AccountNo);
                
                if (exists != null)
                {
                    throw new UserFriendlyException(
                        400,
                        "Duplicate account numbers are not allowed.",
                        "An Account with the same account number already exists."
                    );
                }
        
                entity.AccountName = entity.AccountName.Trim();
                await _repositoryCoaDetail.InsertAsync(entity);
                
                return;
            }
        
            throw new UserFriendlyException(
                400,
                "Duplicate records are not allowed.",
                "An Account Detail with the same name already exists."
            );
        }

        public async Task Delete(Guid id)
        {
            await _repositoryCoaDetail.DeleteAsync(id);
        }

        public async Task<object> ListAll(int pageNo, int pageSize, string query, bool isActive)
        {
            var predicate = PredicateBuilder.New<CoaDetail>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.AccountName.ToLower().Contains(query));
            }

            predicate.And(x => x.Status.ToLower() == (isActive ? "active" : "inactive"));
            
            var totalCount =
                
                await _repositoryCoaDetail.CountAsync(predicate);
            
            var data = await (await _repositoryCoaDetail.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.AccountName, a.AccountNo, a.Status, a.AccountHeaderId, a.AccountHeaderName, a.CurrencyId, a.CurrencyName, a.CurrencyCode, a.CurrentBalance, a.CurrentForeignBalance }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        
        
        public async Task<object> ListActiveAccounts()
        {
            return await _repositoryCoaDetail.GetAllListAsync(x=>x.Status == "Active");
        }
        
        
        public async Task Update(CoaDetail entity)
        {
            var exists = await _repositoryCoaDetail.FirstOrDefaultAsync(
                x => x.AccountNo == entity.AccountNo && x.Id != entity.Id
            );
            
            if (exists != null)
            {
                throw new UserFriendlyException(
                    400,
                    "Account No# already exists",
                    "An Account with the same account numbers already exists."
                );
            }
        
            entity.AccountName = entity.AccountName.Trim();
            await _repositoryCoaDetail.UpdateAsync(entity);

        }
    
}