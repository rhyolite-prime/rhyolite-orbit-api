using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.StudentAttendances;

public interface IStudentAttendanceManager: Abp.Domain.Services.IDomainService
{
    Task<object> GetAll(int pageNo, int pageSize, string query);
    Task Create(StudentAttendance entity);
    Task<string> MarkBiometricAttendance(int tenantId,int biometricId);
    Task Delete(Guid id);

    //enquiry...
    Task<object> ListAll(Guid classId, DateTime startDate, DateTime endDate);
}