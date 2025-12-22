using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.MessageTemplates;

public class MessageTemplateManager : Abp.Domain.Services.DomainService, IMessageTemplateManager
{
    private readonly IRepository<MessageTemplate, Guid> _repositoryMessageTemplate;

    public MessageTemplateManager(IRepository<MessageTemplate, Guid> repositoryMessageTemplate)
    {
        _repositoryMessageTemplate = repositoryMessageTemplate;
    }

    public async Task Create(MessageTemplate entity)
    {
        var datta = await _repositoryMessageTemplate.FirstOrDefaultAsync(x => x.Alias == entity.Alias);

        if (datta == null)
        {
            await _repositoryMessageTemplate.InsertAsync(entity);
            return;
        }
        
        throw new UserFriendlyException(400, "Duplicate records are not allowed.");
    }
    
    public async Task Update(MessageTemplate entity)
    {
        await _repositoryMessageTemplate.UpdateAsync(entity);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<MessageTemplate>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(a => a.Alias.Contains(query));

        }
            
        var totalCount =
                
            await _repositoryMessageTemplate.CountAsync(predicate);
            
        var data = await (await _repositoryMessageTemplate.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Alias, a.MessageContent, a.MessageSubject, a.CreationTime, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
   
    public async Task Delete(Guid id)
    {
        await _repositoryMessageTemplate.DeleteAsync(id);

    }
}