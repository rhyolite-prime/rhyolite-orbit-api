using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.ResultsUploads;

public interface IResultsUploadManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(Guid academicYearId, Guid termId, Guid classId, Guid subjectId, Guid resultTypeId);
    Task<object> ListAll(Guid academicYearId, Guid termId, Guid classId,int tenantId);
    Task Create(ResultsUpload entity);
    Task Update(ResultsUpload entity);
    Task Delete(Guid id);
}