using System;
using System.Threading.Tasks;
using RhyoliteOrbit.DomainServices.SchoolManager.AcademicYears.Dto;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.AcademicYears;

public interface IAcademicYearManager: Abp.Domain.Services.IDomainService
{
    Task<object> GetAll(int pageNo, int pageSize, string query);
    Task<object> GetAsync(Guid id);
    Task Create(AcademicYear entity);
    Task Update(AcademicYear entity);
    Task Delete(Guid id);

    //terms
    Task CreateTerm(TermDto input);
    Task UpdateTerm(TermDto input);
    Task DeleteTerm(Guid academicYearId, Guid id);
}