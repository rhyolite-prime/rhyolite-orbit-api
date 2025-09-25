using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.ProductionPlanning;

namespace RhyoliteOrbit.DomainServices.ProductionPlanning.Products;

public class ProductManager: DomainService, IProductManager
{
    private readonly IRepository<Product, Guid> _repositoryProduct;
        
    public ProductManager(IRepository<Product, Guid> repositoryProduct)
    {
        _repositoryProduct = repositoryProduct;
    }

    public async Task Create(Product entity)
    {
        var datta = await _repositoryProduct.FirstOrDefaultAsync(x => x.Name == entity.Name);

        if (datta == null)
        {
            await _repositoryProduct.InsertAsync(entity);
            return;
        }
        
        throw new UserFriendlyException(400, "Duplicate records are not allowed.");
         
    }

    public async Task Update(Product entity)
    {
        await _repositoryProduct.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryProduct.GetAsync(id);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<Product>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Name.ToLower().Contains(query.ToLower()));
        }
            
        var totalCount =
                
            await _repositoryProduct.CountAsync(predicate);
            
        var data = await (await _repositoryProduct.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.UnitOfMeasure, a.StandardCost, a.SellingPrice, a.CreationTime, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

        var totalPages = totalCount > 0 ? Convert.ToInt32(Math.Ceiling(totalCount / (decimal)pageSize)) : 0;

        var paginatedResult = new
        {
            pageNo,
            pageSize,
            totalCount,
            totalPages,
            data,
            lowerBound = pageSize * (pageNo - 1) + 1,
            upperBound = totalPages == pageNo ? totalCount : pageNo * pageSize,
        };

        return paginatedResult;
    }
    public async Task Delete(Guid id)
    {
        await _repositoryProduct.DeleteAsync(id);

    }
    
}