using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Shared.Banks;
using RhyoliteOrbit.DomainServices.Shared.Banks.Dto;
using RhyoliteOrbit.Models.Shared;
using RhyoliteOrbit.Services.Shared.Banks.Dto;

namespace RhyoliteOrbit.Services.Shared.Banks;

public class BankAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IBankManager _bankManager;
    private readonly IMapper _mapper;

    public BankAppService(IBankManager bankManager, IMapper mapper)
    {
        _bankManager = bankManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _bankManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task Create(CreateBankInput input)
    {
        var obj = _mapper.Map<Bank>(input);
        await _bankManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateBankInput input)
    {
        var obj = _mapper.Map<Bank>(input);
        await _bankManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid Id)
    {   
        await _bankManager.Delete(Id);
    }

    // branches...
    [HttpPost]
    public async Task CreateBranch(BranchDto input)
    {
       await _bankManager.CreateBranch(input);
    }
    
    [HttpPost]
    public async Task UpdateBranch(BranchDto input)
    {
        await _bankManager.UpdateBranch(input);
    }

    [HttpPost]
    public async Task DeleteBranch(BranchDto input)
    {
        await _bankManager.DeleteBranch(input);
    }
}