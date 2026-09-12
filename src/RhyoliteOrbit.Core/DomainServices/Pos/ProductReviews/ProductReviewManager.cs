using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.ProductReviews;

public class ProductReviewManager: DomainService, IProductReviewManager
{
    private readonly IRepository<ProductReview, Guid> _repositoryProductReview;

    public ProductReviewManager(IRepository<ProductReview, Guid> repositoryProductReview)
    {
        _repositoryProductReview = repositoryProductReview;
    }


    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
            
        var predicate = PredicateBuilder.New<ProductReview>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.BranchName.ToLower().Contains(query.ToLower()) || x.CustomerName.ToLower().Contains(query.ToLower()) || x.ItemName.ToLower().Contains(query.ToLower()) || x.CustomerPhoneNo.ToLower().Contains(query.ToLower()));

        }
            
        var totalCount =
                
            await _repositoryProductReview.CountAsync(predicate);
            
        var data = await (await _repositoryProductReview.GetAllAsync()).Where(predicate).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

        var totalPages = totalCount > 0 ? Convert.ToInt32(Math.Ceiling(totalCount / (decimal)pageSize)) : 0;

        var paginatedResult = new
        {
            pageNo,
            totalCount,
            totalPages,
            data,
            lowerBound = pageSize * (pageNo - 1) + 1,
            upperBound = totalPages == pageNo ? totalCount : pageNo * pageSize,
        };

        return paginatedResult;
            
    }


    public async Task Create(ProductReview entity)
    {
        await _repositoryProductReview.InsertAsync(entity);

    }

    public async Task Delete(Guid id)
    {
        await _repositoryProductReview.DeleteAsync(id);
    }
}