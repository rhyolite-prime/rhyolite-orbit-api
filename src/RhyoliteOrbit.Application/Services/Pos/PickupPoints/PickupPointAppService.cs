using System;
using System.Threading.Tasks;
using Abp.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.CloudinaryApi;
using RhyoliteOrbit.DomainServices.Pos.PickupPoints;
using RhyoliteOrbit.Models.Pos;
using RhyoliteOrbit.Services.Pos.PickupPoints.Dto;

namespace RhyoliteOrbit.Services.Pos.PickupPoints;

[AbpAuthorize]
public class PickupPointAppService : RhyoliteOrbitAppServiceBase
{
    private readonly IPickupPointManager _pickupPointManager;
    private readonly IFileUploadApi _fileUploadApi;
    private readonly IMapper _mapper;
    private readonly string _defaultPickupPointLogo = "";


    public PickupPointAppService(IPickupPointManager pickupPointManager, IMapper mapper, IFileUploadApi fileUploadApi)
    {
        _pickupPointManager = pickupPointManager;
        _mapper = mapper;
        _fileUploadApi = fileUploadApi;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageNo"></param>
    /// <param name="pageSize"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    [AbpAuthorize(PermissionNames.POS_ViewPickupPoints)]
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _pickupPointManager.GetAll(pageNo, pageSize, query);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [AbpAuthorize(PermissionNames.POS_CreatePickupPoints)]
    [HttpPost]
    public async Task Create(PickupPointDto dto)
    {
         
        var tenantInfo = await GetCurrentTenantAsync();
        
        var imageUrl = _defaultPickupPointLogo;

        if (!string.IsNullOrEmpty(dto.ImageUrl))
        {
            imageUrl = await _fileUploadApi.SingleUpload(dto.ImageUrl, "pos_repository");
        }

        dto.ImageUrl = imageUrl;
        
        var obj = _mapper.Map<PickupPoint>(dto);
        
        await _pickupPointManager.Create(obj, tenantInfo.DataSupportBusinessAccountId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    [AbpAuthorize(PermissionNames.POS_UpdatePickupPoints)]
    [HttpPost]
    public async Task Update(PickupPointDto dto)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        
        var imageUrl = _defaultPickupPointLogo;

        if (!string.IsNullOrEmpty(dto.ImageUrl))
        {
            imageUrl = await _fileUploadApi.SingleUpload(dto.ImageUrl, "pos_repository");
        }

        dto.ImageUrl = imageUrl;
        
        var obj = _mapper.Map<PickupPoint>(dto);
        obj.TenantId =  tenantInfo.Id;
        await _pickupPointManager.Update(obj, tenantInfo.DataSupportBusinessAccountId);   
    }

    /// <summary>
    /// Deletes a specific pickup point using the provided unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the pickup point to be deleted.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    [AbpAuthorize(PermissionNames.POS_DeletePickupPoints)]
    [HttpDelete]
    public async Task Delete(Guid id)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        
        await _pickupPointManager.Delete(id, tenantInfo.DataSupportBusinessAccountId);
    }
}