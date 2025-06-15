using System;
using System.Threading.Tasks;
using RhyoliteOrbit.DomainServices.Shared.MetaData.Dto;

namespace RhyoliteOrbit.DomainServices.Shared.MetaData;

public interface IAcademicPeriodMetaDataManager : Abp.Domain.Services.IDomainService
{
    Task<BasicAcademicInfo> GetAcademicPeriodMeta(Guid academicYearId, Guid termId);
    Task<BasicAcademicInfo> GetStudentMeta(Guid studentId, Guid classId);
    Task<BasicAcademicInfo> GetAcademicPeriodMeta(Guid academicYearId, Guid termId, Guid classId);
    Task<BasicAcademicInfo> GetAcademicPeriodMeta(Guid academicYearId, Guid termId, Guid classId, Guid studentId);
}