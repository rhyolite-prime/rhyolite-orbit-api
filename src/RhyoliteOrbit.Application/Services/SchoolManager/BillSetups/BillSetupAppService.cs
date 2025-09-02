using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.SchoolManager.BillSetups;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Services.SchoolManager.BillSetups.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.BillSetups;

public class BillSetupAppService: RhyoliteOrbitAppServiceBase
{
    
    private readonly IBillSetupManager  _billSetupManager;
    private readonly IMapper _mapper;

    public BillSetupAppService(IBillSetupManager billSetupManager, IMapper mapper)
    {
        _billSetupManager = billSetupManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query, Guid academicYearId, Guid termId, Guid classId, Guid billTypeId)
    {
        return await _billSetupManager.ListAll(pageNo, pageSize, query, academicYearId, termId, classId, billTypeId);
    }

    [HttpGet]
    public async Task<object> GetAsync(Guid academicYearId, Guid termId, Guid classId, Guid billTypeId)
    {
        return await _billSetupManager.GetAsync(academicYearId, termId, classId, billTypeId);
    }

    [HttpPost]
    public async Task<object> Create(CreateBillSetupInput input)
    {
        var obj = _mapper.Map<BillSetup>(input);
        return await _billSetupManager.Create(obj);
    }

    [HttpDelete]
    public async Task DeleteDetail(Guid id, Guid headerId)
    {
        await _billSetupManager.DeleteDetail(id, headerId);
    }   

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _billSetupManager.Delete(id);
    }
    
}