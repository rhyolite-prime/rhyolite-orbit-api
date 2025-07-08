using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.LoanTypes;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.LoanTypes.Dto;

namespace RhyoliteOrbit.Services.Payroll.LoanTypes;

public class LoanTypeAppService: RhyoliteOrbitAppServiceBase
{
    private readonly ILoanTypeManager _loanTypeManager;
    private readonly IMapper _mapper;

    public LoanTypeAppService(ILoanTypeManager loanTypeManager, IMapper mapper)
    {
        _loanTypeManager = loanTypeManager;
        _mapper = mapper;
    }
    

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _loanTypeManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task<object> Create(CreateLoanTypeInput input)
    {
        var obj = _mapper.Map<LoanType>(input);
        return await _loanTypeManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateLoanTypeInput input)
    {
        var obj = _mapper.Map<LoanType>(input);
        await _loanTypeManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _loanTypeManager.Delete(id);
    }
}