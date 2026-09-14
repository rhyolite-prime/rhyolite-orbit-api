using System;
using System.Threading.Tasks;
using Abp.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.Pos.Customers;
using RhyoliteOrbit.Models.Pos;
 
using RhyoliteOrbit.Services.Pos.Customers.Dto;

namespace RhyoliteOrbit.Services.Pos.Customers;


public class CustomerAppService: RhyoliteOrbitAppServiceBase
{
    private readonly ICustomerManager _customerManager;
    private readonly IMapper _mapper;
    private readonly IValidateAuth _validateAuth;

    public CustomerAppService(ICustomerManager customerManager, IMapper mapper, IValidateAuth validateAuth)
    {
        _customerManager = customerManager;
        _mapper = mapper;
        _validateAuth = validateAuth;
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageNo"></param>
    /// <param name="pageSize"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    [AbpAuthorize(PermissionNames.POS_ViewCustomerRanks)]
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _customerManager.GetAll(pageNo, pageSize, query);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [AbpAuthorize(PermissionNames.POS_CreateCustomerRanks)]
    [HttpPost]
    public async Task Create(PosCustomerDto dto)
    {
        var obj = _mapper.Map<PosCustomer>(dto);
        await _customerManager.Create(obj);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="id"></param>
    [AbpAuthorize(PermissionNames.POS_UpdateCustomerRanks)]
    [HttpPut("{id}")]
    public async Task Update(PosCustomerDto dto, Guid id)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        
        var obj = _mapper.Map<PosCustomer>(dto);
        obj.TenantId = tenantInfo.Id;
        obj.Id = id;
        await _customerManager.Update(obj);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    [AbpAuthorize(PermissionNames.POS_DeleteCustomerRanks)]
    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _customerManager.DeactivateCustomer(id);
    }
    
    //reserved for integration
    [HttpPost]
    public async Task Register(PosCustomerDto dto)
    {
        var tenantId = await _validateAuth.ValidateTenant();
        //var customerId = await _validateAuth.ValidateCustomer();
        var obj = _mapper.Map<PosCustomer>(dto);
        
        await _customerManager.RegisterCustomer(obj, tenantId);
    }
    
    [HttpPost]
    public async Task<object> Login(PosCustomerLoginDto dto)
    {
        var tenantId = await _validateAuth.ValidateTenant();
        //var customerId = await _validateAuth.ValidateCustomer();
        
       return await _customerManager.CustomerLogin(dto.AccountIdentifier, dto.Password, tenantId);
    }
}