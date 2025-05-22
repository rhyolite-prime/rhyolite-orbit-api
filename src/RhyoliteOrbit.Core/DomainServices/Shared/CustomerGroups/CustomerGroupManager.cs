using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.CustomerGroups;

public class CustomerGroupManager: Abp.Domain.Services.DomainService, ICustomerGroupManager
{
    private readonly IRepository<CustomerGroup, Guid> _repositoryCustomerGroup;

    public CustomerGroupManager(IRepository<CustomerGroup, Guid> repositoryCustomerGroup)
    {
        _repositoryCustomerGroup = repositoryCustomerGroup;
    }

    public async Task Create(CustomerGroup entity)
    {
        var datta = await _repositoryCustomerGroup.FirstOrDefaultAsync(x => x.Name == entity.Name);

        if (datta == null)
        {
            await _repositoryCustomerGroup.InsertAsync(entity);

        }
        
        throw new UserFriendlyException(400, "Duplicate records are not allowed.");
    }
    
    public async Task Update(CustomerGroup entity)
    {
        await _repositoryCustomerGroup.UpdateAsync(entity);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<CustomerGroup>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(a => a.Name.Contains(query));

        }
            
        var totalCount =
                
            await _repositoryCustomerGroup.CountAsync(predicate);
            
        var data = await (await _repositoryCustomerGroup.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.CreationTime, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryCustomerGroup.DeleteAsync(id);

    }
}