using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.CloudinaryApi;
using RhyoliteOrbit.DomainServices.Pos.Merchants;
using RhyoliteOrbit.Models.Pos;
using RhyoliteOrbit.Services.Pos.Merchants.Dto;

namespace RhyoliteOrbit.Services.Pos.Merchants;

[AbpAuthorize]
public class MerchantAppService : RhyoliteOrbitAppServiceBase
{
    private readonly IMerchantManager _merchantManager;
    private readonly IFileUploadApi _fileUploadApi;
    private readonly string _defaultMerchantAvatar = "https://res.cloudinary.com/rhyoliteprime/image/upload/v1749257247/pos/images/shopify_colored-02-512.webp";
    private readonly IMapper _mapper;

    public MerchantAppService(IMerchantManager manufacturerManager, IMapper mapper, IFileUploadApi fileUploadApi)
    {
        _merchantManager = manufacturerManager;
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
    [AbpAuthorize(PermissionNames.POS_ViewMerchants)]
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _merchantManager.GetAll(pageNo, pageSize, query);
    }
    
    [HttpGet]
    public async Task<object> GetCreditReputation(Guid id)
    {
        return await _merchantManager.GetCreditReputation(id);
    }
    
    [AbpAuthorize(PermissionNames.POS_CreateMerchant)]
    [HttpPost]
    public async Task Create(MerchantDto dto)
    {
        var logoUrl = _defaultMerchantAvatar;

        if (!string.IsNullOrEmpty(dto.LogoUrl))
        {
            logoUrl = await _fileUploadApi.SingleUpload(dto.LogoUrl, "pos_repository");
        }

        dto.LogoUrl = logoUrl;
        var obj = _mapper.Map<Merchant>(dto);
        await _merchantManager.Create(obj);
    }
    
    [AbpAuthorize(PermissionNames.POS_UpdateMerchant)]
    [HttpPut("{id}")]
    public async Task UpdateBasicInfo(MerchantDto input, Guid id)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        
        var obj = _mapper.Map<Merchant>(input);
        obj.TenantId = tenantInfo.Id;
        obj.Id = id;
        await _merchantManager.UpdateBasicInfo(obj);
    }
    
    [AbpAuthorize(PermissionNames.POS_UpdateMerchant)]
    [HttpPut]
    public async Task UpdateComplianceDocuments(UpdateMerchantComplianceDocumentDto dto)
    {
        if (dto.ComplianceDocuments.Any())
        {
            foreach (var doc in dto.ComplianceDocuments)
            {
                if (!string.IsNullOrEmpty(doc.DocumentUrl))
                {
                    doc.DocumentUrl = await _fileUploadApi.SingleUpload(doc.DocumentUrl , "pos_repository");
                }
            }
            
             
        }

        await _merchantManager.UpdateComplianceDocuments(dto.Id, dto.ComplianceDocuments);
    }
    
    [AbpAuthorize(PermissionNames.POS_UpdateMerchant)]
    [HttpPut]
    public async Task UpdateRemittanceAccounts(UpdateMerchantRemittanceAccountsDto dto)
    {
        await _merchantManager.UpdateRemittanceAccounts(dto.Id, dto.RemittanceAccounts);
    }
    
    [AbpAuthorize(PermissionNames.POS_UpdateMerchant)]
    [HttpPut]
    public async Task UpdateLogo(UpdateMerchantLogoDto dto)
    {
        var logoUrl = _defaultMerchantAvatar;

        if (!string.IsNullOrEmpty(dto.LogoUrl))
        {
            logoUrl = await _fileUploadApi.SingleUpload(dto.LogoUrl, "pos_repository");
        }

        dto.LogoUrl = logoUrl;
        
        await _merchantManager.UpdateLogo(dto.Id, dto.LogoUrl); 
    }
    
    [AbpAuthorize(PermissionNames.POS_DeleteMerchant)]
    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _merchantManager.Delete(id);
    }
}