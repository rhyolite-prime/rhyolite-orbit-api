using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.PersonnelStatuses;

public class PersonnelStatusManager: Abp.Domain.Services.DomainService, IPersonnelStatusManager
{
    private readonly IRepository<PersonnelStatus, Guid> _repositoryPersonnelStatus;

    public PersonnelStatusManager(IRepository<PersonnelStatus, Guid> repositoryPersonnelStatus)
    {
        _repositoryPersonnelStatus = repositoryPersonnelStatus;
    }

    public async Task Create(PersonnelStatus entity)
    {
        var trimmedName = entity.Name.Trim().ToLowerInvariant();

        var datta = await _repositoryPersonnelStatus.FirstOrDefaultAsync(x => x.Name.Trim().ToLower() == trimmedName);

        if (datta != null)
        {
            throw new UserFriendlyException(400, "Duplicate records are not allowed");
        }

        if (entity.IsDefault)
        {
            var allDefaults = await _repositoryPersonnelStatus.GetAllListAsync(x => x.IsDefault);

            foreach (var defaultRecord in allDefaults)
            {
                defaultRecord.IsDefault = false;
                await _repositoryPersonnelStatus.InsertAsync(entity);
            }
        }

        await _repositoryPersonnelStatus.InsertAsync(entity);
    }
    
    public async Task Update(PersonnelStatus entity)
    {
        await _repositoryPersonnelStatus.UpdateAsync(entity);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<PersonnelStatus>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(a => a.Name.Contains(query));

        }
            
        var totalCount =
                
            await _repositoryPersonnelStatus.CountAsync(predicate);
            
        var data = await (await _repositoryPersonnelStatus.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.CreationTime, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryPersonnelStatus.DeleteAsync(id);

    }
}