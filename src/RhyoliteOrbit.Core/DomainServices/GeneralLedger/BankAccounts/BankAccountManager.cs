using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.GeneralLedger;

namespace RhyoliteOrbit.DomainServices.GeneralLedger.BankAccounts;

public class BankAccountManager: DomainService, IBankAccountManager
{
        private readonly IRepository<BankAccount, Guid> _repositoryBankAccount;

        public BankAccountManager(IRepository<BankAccount, Guid> repositoryBankAccount)
        {
            _repositoryBankAccount = repositoryBankAccount;
        }

        public async Task Create(BankAccount entity)
        {
            var datta = await _repositoryBankAccount.FirstOrDefaultAsync(x => x.BankId == entity.BankId && x.AccountType == entity.AccountType && x.AccountNo == entity.AccountNo);

            if (datta == null)
            {
                await _repositoryBankAccount.InsertAsync(entity);

                return;
            }

            throw new UserFriendlyException(
                400,
                "Duplicate records are not allowed.",
                "An Academic Year with the same name already exists."
            );
            
        }

        public async Task Update(BankAccount entity)
        {
            await _repositoryBankAccount.UpdateAsync(entity);
        }

        public async Task<object> GetAsync(Guid id)
        {
            return await _repositoryBankAccount.FirstOrDefaultAsync(id);
        }

        public async Task<object> ListAll(int pageNo, int pageSize, string query)
        {
            var predicate = PredicateBuilder.New<BankAccount>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.AccountName.ToLower().Contains(query.ToLower()));
            }
            
            var totalCount =  await _repositoryBankAccount.CountAsync(predicate);
            
            var data = await (await _repositoryBankAccount.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.AccountName, a.AccountNo, a.AccountId, a.AccountType, a.ContactNo, a.ContactPerson, a.BankId, a.BankName, a.BankBranchId, a.BankBranchName, a.Address, a.Location, a.CurrencyId, a.CurrencyCode, a.CurrencyName, a.ChequeBookStart, a.ChequeBookEnd, DateOpened = a.DateOpened.ToString("dd-MM-yyyy"), }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
            await _repositoryBankAccount.DeleteAsync(id);

        }
}