using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.BillTypes;

public class BillTypeManager: Abp.Domain.Services.DomainService, IBillTypeManager
{
    private readonly IRepository<BillType, Guid> _repositoryBillType;
        
    public BillTypeManager(IRepository<BillType, Guid> repositoryBillType)
    {
        _repositoryBillType = repositoryBillType;
    }

    public async Task<object> Create(BillType entity)
    {
        var datta = await _repositoryBillType.FirstOrDefaultAsync(x => x.Name == entity.Name);

        if (datta == null)
        {
            await _repositoryBillType.InsertAsync(entity);

            return new
            {
                code = 200,
                message = "successful"
            };
        }
        
        throw new UserFriendlyException(400, "Duplicate records are not allowed.");
         
    }

    public async Task Update(BillType entity)
    {
        await _repositoryBillType.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryBillType.GetAsync(id);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<BillType>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Name.Contains(query));
        }
            
        var totalCount =
                
            await _repositoryBillType.CountAsync(predicate);
            
        var data = await (await _repositoryBillType.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.CreationTime, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryBillType.DeleteAsync(id);

    }
}