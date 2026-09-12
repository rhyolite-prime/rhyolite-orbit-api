using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.CreditTerms;

public class CreditTermManager: DomainService, ICreditTermManager
{
    private readonly IRepository<CreditTerm, Guid> _repositoryCreditTerm;

    public CreditTermManager(IRepository<CreditTerm, Guid> repositoryCreditTerm)
    {
        _repositoryCreditTerm = repositoryCreditTerm;
    }

    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<CreditTerm>();

        predicate.And(x => !x.IsDeleted);
        
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Name.ToLower().Contains(query.ToLower()));

        }
        
        var totalCount =
            
            await _repositoryCreditTerm.CountAsync(predicate);
        
        var data = await (await _repositoryCreditTerm.GetAllAsync()).Where(predicate).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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

    public async Task Create(CreditTerm entity)
    {

        if (string.IsNullOrWhiteSpace(entity.Name))
        {
            throw new UserFriendlyException(400, "Credit term name is required.");
        }

        var trimmedName = entity.Name.Trim();

        var creditTermInfo = await _repositoryCreditTerm.FirstOrDefaultAsync(x=> x.Name.ToLower() == trimmedName.ToLower());

        if (creditTermInfo != null)
        {
            throw new UserFriendlyException(400, $"A credit term named '{trimmedName}' already exists.");
        }

        entity.Name = trimmedName;
        await _repositoryCreditTerm.InsertAsync(entity);
    }

    public async Task Update(CreditTerm entity)
    {
        await _repositoryCreditTerm.UpdateAsync(entity);
    }

    public async Task Delete(Guid id)
    {
        await _repositoryCreditTerm.DeleteAsync(id);
    }
}