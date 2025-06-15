using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.TerminalReports;

public interface ITerminalReportManager: Abp.Domain.Services.IDomainService
{
    Task<object> GetAll(int pageNo, int pageSize, string query);
    Task<object> GetAsync(Guid id);
    Task<object> Create(TerminalReport entity);
    Task Update(TerminalReport entity);
    Task Delete(Guid id);
}