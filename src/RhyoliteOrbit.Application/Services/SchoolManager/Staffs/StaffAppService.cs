using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.SchoolManager.Staffs;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Services.SchoolManager.Staffs.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.Staffs;

public class StaffAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IStaffManager _staffManager;
    private readonly IValidateAuth _validateAuth;
    private readonly IMapper _mapper;

    public StaffAppService(IStaffManager staffManager, IMapper mapper, IValidateAuth validateAuth)
    {
        _staffManager = staffManager;
        _mapper = mapper;
        _validateAuth = validateAuth;
    }

     
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _staffManager.ListAll(pageNo, pageSize, query);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    [HttpPost]
    public async Task Create(CreateStaffInput input)
    {
        var obj = _mapper.Map<Staff>(input);
        await _staffManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateStaffInput input)
    {
        var obj = _mapper.Map<Staff>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _staffManager.Update(obj);
    }


    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _staffManager.Delete(id);
    }
    
    //self service ... 
    [HttpPost]
    public async Task Register(CreateStaffInput input)
    {
        var tenantId = await _validateAuth.ValidateTenant();
        var obj = _mapper.Map<Staff>(input);
        obj.TenantId = tenantId;
        await _staffManager.RegisterViaSelfService(obj);
    }
    
    
    //verify staff 
    [HttpGet]
    public async Task VerifyStaff(string staffIdentifier, string pin)
    {
        await Task.Delay(1000);
    }
}