using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.ProductionPlanning.Products;
using RhyoliteOrbit.Models.ProductionPlanning;
using RhyoliteOrbit.Services.ProductionPlanning.Products.Dto;

namespace RhyoliteOrbit.Services.ProductionPlanning.Products;

public class ProductAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IProductManager  _productManager;
    private readonly IMapper _mapper;

    public ProductAppService(IProductManager productManager, IMapper mapper)
    {
        _productManager = productManager;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _productManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task Create(CreateProductInput input)
    {
        var obj = _mapper.Map<Product>(input);
        await _productManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateProductInput input)
    {
        var obj = _mapper.Map<Product>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _productManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _productManager.Delete(id);
    }
}