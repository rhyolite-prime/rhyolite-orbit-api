using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.DomainServices.Shared.Banks.Dto;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.Banks;

public class BankManager: DomainService, IBankManager
{
    private readonly IRepository<Bank, Guid> _repositoryBank;

        public BankManager(IRepository<Bank, Guid> repositoryBank)
        {
            _repositoryBank = repositoryBank;
        }

        public async Task<object> Create(Bank entity)
        {
            var datta = await _repositoryBank.FirstOrDefaultAsync(x => x.Name.Trim().ToLower() == entity.Name.Trim().ToLower());

            if (datta == null)
            {
                entity.Name = entity.Name.Trim();

                await _repositoryBank.InsertAsync(entity);

                return new
                {
                    code = 200,
                    message = "successful"
                };
            }

            throw new UserFriendlyException(
                400,
                "Duplicate records are not allowed.",
                "A Bank with the same name already exists."
            );
            
        }

        public async Task Update(Bank entity)
        {
            entity.Name = entity.Name.Trim();

            await _repositoryBank.UpdateAsync(entity);
        }

        public async Task<object> ListAll(int pageNo, int pageSize, string query)
        {
            var predicate = PredicateBuilder.New<Bank>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.Name.Contains(query));
            }
            
            var totalCount =
                
                await _repositoryBank.CountAsync(predicate);
            
            var data = await (await _repositoryBank.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.BankCode, a.BankBranches,   }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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

        public async Task<Bank> GetAsync(string bankName)
        {
            return await _repositoryBank.FirstOrDefaultAsync(x=> !string.IsNullOrEmpty(bankName) && x.Name.ToLower() == bankName.Trim().ToLower() );

        }

        public async Task Delete(Guid id)
        {
            await _repositoryBank.DeleteAsync(id);

        }

        // branches
        public async Task<object> CreateBranch(BranchDto input)
        {
            var bankData = await _repositoryBank.FirstOrDefaultAsync(input.BankId);
            var branchList = bankData.BankBranches;
            var branchInfo = branchList.FirstOrDefault(x => x.Id == input.Id);

            if (branchInfo == null)
            {
                bankData.BankBranches.Add(new BankBranch
                {
                    Id = input.Id,
                    ContactNo = input.ContactNo,
                    ContactPerson = input.ContactPerson,
                    Email = input.Email,
                    Name = input.Name,

                });

                await _repositoryBank.UpdateAsync(bankData);

                return new
                {
                    code = 200,
                    message = "successful"
                };
            }
            else
            {
                return new
                {
                    code = 400,
                    message = "Duplicate records are not allowed."
                };

            }

        }


        public async Task UpdateBranch(BranchDto input)
        {
            var bankData = await _repositoryBank.FirstOrDefaultAsync(input.BankId);
            var branchList = bankData.BankBranches;
            var branchInfo = branchList.FirstOrDefault(x => x.Id == input.Id);
            branchList.Remove(branchInfo);

            branchInfo.Name = input.Name;
            branchInfo.ContactPerson = input.ContactPerson;
            branchInfo.Email = input.Email;
            branchInfo.ContactNo = input.ContactNo;

            branchList.Add(branchInfo);

            bankData.BankBranches = branchList;

            await _repositoryBank.UpdateAsync(bankData);
        }


        public async Task DeleteBranch(BranchDto input)
        {
            var bankData = await _repositoryBank.FirstOrDefaultAsync(input.BankId);
            var branchList = bankData.BankBranches;
            var branchInfo = branchList.FirstOrDefault(x => x.Id == input.Id);
            branchList.Remove(branchInfo);

            bankData.BankBranches = branchList;

            await _repositoryBank.UpdateAsync(bankData);

        }
}