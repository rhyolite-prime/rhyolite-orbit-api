using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.StaffDesignations;

public interface IStaffDesignationManager: Abp.Domain.Services.IDomainService
{
    Task<object> GetAll(int pageNo, int pageSize, string query);
    Task<object> Create(StaffDesignation entity);
    Task Update(StaffDesignation entity);
    Task Delete(Guid id);
}