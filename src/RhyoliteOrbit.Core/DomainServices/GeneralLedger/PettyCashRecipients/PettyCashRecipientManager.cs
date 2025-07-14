using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.GeneralLedger;

namespace RhyoliteOrbit.DomainServices.GeneralLedger.PettyCashRecipients;

public class PettyCashRecipientManager: DomainService, IPettyCashRecipientManager
{
    private readonly IRepository<PettyCashRecipient, Guid> _repositoryPettyCashRecipient;

    public PettyCashRecipientManager(IRepository<PettyCashRecipient, Guid> repositoryPettyCashRecipient)
    {
        _repositoryPettyCashRecipient = repositoryPettyCashRecipient;
    }

    public async Task Create(PettyCashRecipient entity)
    {
        var datta = await _repositoryPettyCashRecipient.FirstOrDefaultAsync(x => x.RecipientName == entity.RecipientName);

        if (datta == null)
        {
            await _repositoryPettyCashRecipient.InsertAsync(entity);

            return;
        }
        
        throw new UserFriendlyException(
            400,
            "Duplicate records are not allowed.",
            "A Petty Cash Recipient with the same name already exists."
        );
    }

    public async Task Update(PettyCashRecipient entity)
    {
        await _repositoryPettyCashRecipient.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryPettyCashRecipient.FirstOrDefaultAsync(id);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<PettyCashRecipient>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.RecipientName.ToLower().Contains(query));
        }
            
        var totalCount =
                
            await _repositoryPettyCashRecipient.CountAsync(predicate);
            
        var data = await (await _repositoryPettyCashRecipient.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.RecipientName, a.PhoneNo, a.Email }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryPettyCashRecipient.DeleteAsync(id);

    }
}