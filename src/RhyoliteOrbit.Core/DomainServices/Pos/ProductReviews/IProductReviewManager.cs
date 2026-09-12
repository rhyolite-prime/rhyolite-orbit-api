using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.ProductReviews;

public interface IProductReviewManager:IDomainService
{
    Task<object> GetAll(int pageNo, int pageSize, string query);
    Task Create(ProductReview entity);
    Task Delete(Guid id);
}