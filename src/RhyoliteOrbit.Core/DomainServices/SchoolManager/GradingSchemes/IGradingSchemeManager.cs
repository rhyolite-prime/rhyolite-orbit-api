using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.GradingSchemes;

public interface IGradingSchemeManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(Guid academicYearId, Guid termId, Guid levelId);
    Task<object> ListAll(int pageNo, int pageSize, Guid academicYearId, Guid termId, Guid levelId);
    Task Create(GradingScheme input);
    Task Update(GradingScheme input);
    Task Delete(Guid id);
}