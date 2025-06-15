using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.StaffDesignations;

public class StaffDesignationManager: Abp.Domain.Services.DomainService, IStaffDesignationManager
{
    private readonly IRepository<StaffDesignation, Guid> _repositoryStaffDesignation;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    public StaffDesignationManager(IRepository<StaffDesignation, Guid> repositoryStaffDesignation, IUnitOfWorkManager unitOfWorkManager)
    {
        _repositoryStaffDesignation = repositoryStaffDesignation;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task<object> Create(StaffDesignation entity)
    {
        var datta = await _repositoryStaffDesignation.FirstOrDefaultAsync(x => x.Name == entity.Name);
        if (datta == null)
        {

            await _repositoryStaffDesignation.InsertAsync(entity);
            return new
            {
                code = 200,
                message = "successful"
            };
        }

        throw new UserFriendlyException(400, "Duplicate records are not allowed.");
        
    }


    public async Task CreateBulk(List<StaffDesignation> inputs, int tenantId)
    {

        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            foreach (var entity in inputs)
            {
                entity.TenantId = tenantId;

                await _repositoryStaffDesignation.InsertAsync(entity);

            }

        }

    }

    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<StaffDesignation>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Name.Contains(query));
        }
            
        var totalCount =
                
            await _repositoryStaffDesignation.CountAsync(predicate);
            
        var data = await (await _repositoryStaffDesignation.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryStaffDesignation.DeleteAsync(id);

    }

    public async Task Update(StaffDesignation entity)
    {
        await _repositoryStaffDesignation.UpdateAsync(entity);
    }
}