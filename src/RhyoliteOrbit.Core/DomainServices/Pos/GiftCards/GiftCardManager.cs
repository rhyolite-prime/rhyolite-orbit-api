using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.GiftCards;

public class GiftCardManager:DomainService, IGiftCardManager
{
    private readonly IRepository<GiftCard, Guid> _repositoryGiftCard;

    public GiftCardManager(IRepository<GiftCard, Guid> repositoryGiftCard)
    {
        _repositoryGiftCard = repositoryGiftCard;
    }
    
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<GiftCard>();

        predicate.And(x => !x.IsDeleted);
        
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Code.ToLower().Contains(query.ToLower()));
        }
        
        var totalCount =
            
            await _repositoryGiftCard.CountAsync(predicate);
        
        var data = await (await _repositoryGiftCard.GetAllAsync()).Where(predicate).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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


    public async Task Create(GiftCard entity)
    {
        var datta = await _repositoryGiftCard.FirstOrDefaultAsync(x => x.Code == entity.Code);
        if (datta != null)
        {
            throw new UserFriendlyException(400, "Duplicate records are not allowed.");
        }
        
        await _repositoryGiftCard.InsertAsync(entity);

    }


    public async Task Update(GiftCard entity)
    {
        await _repositoryGiftCard.UpdateAsync(entity);
    }

    public async Task Delete(Guid id)
    {
        await _repositoryGiftCard.DeleteAsync(id);
    }
}