using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.ProductionPlanning;

namespace RhyoliteOrbit.DomainServices.ProductionPlanning.RawMaterials;

public class RawMaterialManager: DomainService, IRawMaterialManager
{
    private readonly IRepository<RawMaterial, Guid> _repositoryRawMaterial;
        
    public RawMaterialManager(IRepository<RawMaterial, Guid> repositoryRawMaterial)
    {
        _repositoryRawMaterial = repositoryRawMaterial;
    }

    public async Task Create(RawMaterial entity)
    {
        var datta = await _repositoryRawMaterial.FirstOrDefaultAsync(x => x.Name == entity.Name);

        if (datta == null)
        {
            await _repositoryRawMaterial.InsertAsync(entity);
            return;
        }
        
        throw new UserFriendlyException(400, "Duplicate records are not allowed.");
         
    }

    public async Task Update(RawMaterial entity)
    {
        await _repositoryRawMaterial.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryRawMaterial.GetAsync(id);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<RawMaterial>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Name.ToLower().Contains(query.ToLower()));
        }
            
        var totalCount =
                
            await _repositoryRawMaterial.CountAsync(predicate);
            
        var data = await (await _repositoryRawMaterial.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.UnitOfMeasure, a.StandardCost, a.CreationTime, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryRawMaterial.DeleteAsync(id);

    }
}