using System;
using System.Threading.Tasks;
using RhyoliteOrbit.DomainServices.Shared.Banks.Dto;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.Banks;

public interface IBankManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> Create(Bank input);
    Task<Bank> GetAsync(string bankName);
    Task Update(Bank input);
    Task Delete(Guid id);

    //branches
    Task<object> CreateBranch(BranchDto input);
    Task UpdateBranch(BranchDto input);
    Task DeleteBranch(BranchDto input);
}