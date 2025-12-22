using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RhyoliteOrbit.DomainServices.SchoolManager.Students.Dto;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.Students;

public interface IStudentManager: Abp.Domain.Services.IDomainService
{
    Task<object> GetAll(int pageNo, int pageSize, string query, Guid classId);
    Task<object> GetAsync(Guid id);
    Task<object> ListAllByClass(Guid id);
    Task<IEnumerable<object>> ListStudsByClass(Guid id);
    Task<object> Create(Student entity);
    Task<object> Update(Student entity);
    Task Promote(StudentPromotion studentPromotion);
    Task PromoteAlumni(AlumniStudentPromotion studentPromotion, DateTime completionDate);
    Task Delete(Guid id);
    Task<Guid> GetStudentId(string studentId);
    Task<object> ListStudentsForPromotion(Guid classId);

    //Enquiry
    Task<object> EnqStudsByClass(Guid classId);
    Task<object> EnqStudsByNationality(Guid nationalityId);
    Task<object> EnqStudsByReligion(string religion);

    //exports
    Task<IEnumerable<object>> AttitudeExcelExport(Guid classId);
    Task<IEnumerable<object>> ConductsExcelExport(Guid classId);
    Task<IEnumerable<object>> TeacherRemarksExcelExport(Guid classId);
    Task<IEnumerable<object>> ResultsUploadExcelExport(Guid classId);
    Task<IEnumerable<object>> OpeningBalanceExcelExport(Guid classId);
        
    //external api
    Task AssignPinsToAllStudents();
    Task<object> GetStudentInfo(string studentId, string pin, int tenantId);
    Task BiometricEnrollment(int tenantId, int biometricId, string studentId);
}