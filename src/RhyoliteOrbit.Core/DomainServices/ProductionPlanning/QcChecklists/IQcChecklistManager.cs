using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.ProductionPlanning;

namespace RhyoliteOrbit.DomainServices.ProductionPlanning.QcChecklists;

public interface IQcChecklistManager: IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task Create(QcChecklist entity);
    Task Update(QcChecklist entity);
    Task Delete(Guid id);
}