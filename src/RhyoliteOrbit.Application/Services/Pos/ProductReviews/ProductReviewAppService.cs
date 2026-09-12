using System;
using System.Threading.Tasks;
using Abp.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.Pos.ProductReviews;
using RhyoliteOrbit.Models.Pos;
using RhyoliteOrbit.Services.Pos.ProductReviews.Dto;

namespace RhyoliteOrbit.Services.Pos.ProductReviews;

[AbpAuthorize]
public class ProductReviewAppService : RhyoliteOrbitAppServiceBase
{
    private readonly IProductReviewManager _productReviewManager;
    private readonly IValidateAuth _validateAuth;
    private readonly IMapper _mapper;

    public ProductReviewAppService(IProductReviewManager productReviewManager, IMapper mapper, IValidateAuth validateAuth)
    {
        _productReviewManager = productReviewManager;
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
    [AbpAuthorize(PermissionNames.POS_ViewProductReviews)]
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _productReviewManager.GetAll(pageNo, pageSize, query);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task Create(ProductReviewDto dto)
    {
        var tenantId = await _validateAuth.ValidateTenant();
        
        var obj = _mapper.Map<ProductReview>(dto);
        obj.TenantId = tenantId;
        await _productReviewManager.Create(obj);
    }
    
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    [AbpAuthorize(PermissionNames.POS_DeleteProductReview)]
    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _productReviewManager.Delete(id);
    }
}