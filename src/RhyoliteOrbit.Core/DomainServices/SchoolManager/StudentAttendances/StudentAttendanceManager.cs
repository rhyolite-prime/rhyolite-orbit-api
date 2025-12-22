using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Expressions;
using HashidsNet;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.DomainServices.Shared.MetaData;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.StudentAttendances;

public class StudentAttendanceManager: Abp.Domain.Services.DomainService, IStudentAttendanceManager
{
        private readonly IRepository<StudentAttendance, Guid> _repositoryStudentAttendance;
        private readonly IRepository<SchoolProfile, Guid> _repositorySchoolProfile;
        private readonly IRepository<Student, Guid> _repositoryStudent;
        private readonly IAcademicPeriodMetaDataManager _academicPeriodMetaDataManager;
        private readonly IHashids _hashids;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public StudentAttendanceManager(IRepository<StudentAttendance, Guid> repositoryStudentAttendance, IRepository<SchoolProfile, Guid> repositorySchoolProfile, IUnitOfWorkManager unitOfWorkManager, IRepository<Student, Guid> repositoryStudent, IAcademicPeriodMetaDataManager academicPeriodMetaDataManager, IHashids hashids)
        {
            _repositoryStudentAttendance = repositoryStudentAttendance;
            _repositorySchoolProfile = repositorySchoolProfile;
            _unitOfWorkManager = unitOfWorkManager;
            _repositoryStudent = repositoryStudent;
            _academicPeriodMetaDataManager = academicPeriodMetaDataManager;
            _hashids = hashids;
        }


        
        public async Task Create(StudentAttendance entity)
        {
            // Get current school profile
            var schoolProfile = await (await _repositorySchoolProfile.GetAllAsync()).FirstOrDefaultAsync();
            if (schoolProfile == null)
            {
                return; // No profile means we can’t assign academic context
            }

            // Get existing attendance record for the same date and class
            var existingAttendance = await _repositoryStudentAttendance.FirstOrDefaultAsync(
                x => x.AttendanceDate.Date == entity.AttendanceDate.Date && x.ClassId == entity.ClassId);

            // Get academic metadata (year, term, class names)
            var metaData = await _academicPeriodMetaDataManager.GetAcademicPeriodMeta(
                schoolProfile.CurrentAcademicYearId,
                schoolProfile.CurrentTermId,
                entity.ClassId);

            if (existingAttendance != null)
            {
                // Update existing record
                existingAttendance.AcademicYearId = schoolProfile.CurrentAcademicYearId;
                existingAttendance.TermId = schoolProfile.CurrentTermId;
                existingAttendance.ClassId = entity.ClassId;
                existingAttendance.NoPresent = entity.NoPresent;
                existingAttendance.AttendanceDate = entity.AttendanceDate;
                existingAttendance.Details = entity.Details;

                if (metaData != null)
                {
                    existingAttendance.AcademicYearName = metaData.AcademicYearName;
                    existingAttendance.TermName = metaData.TermName;
                    existingAttendance.ClassName = metaData.ClassName;
                }

                await _repositoryStudentAttendance.UpdateAsync(existingAttendance);
            }
            else
            {
                // Assign academic context for new record
                entity.AcademicYearId = schoolProfile.CurrentAcademicYearId;
                entity.TermId = schoolProfile.CurrentTermId;

                if (metaData != null)
                {
                    entity.AcademicYearName = metaData.AcademicYearName;
                    entity.TermName = metaData.TermName;
                    entity.ClassName = metaData.ClassName;
                }

                await _repositoryStudentAttendance.InsertAsync(entity);
            }
        }
        

        public async Task Delete(Guid id)
        {
            await _repositoryStudentAttendance.DeleteAsync(id);
        }

        public async Task<object> GetAll(int pageNo, int pageSize, string query)
        {
            var predicate = PredicateBuilder.New<StudentAttendance>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.ClassName.Contains(query) || x.AcademicYearName.Contains(query) || x.TermName.Contains(query));
            }
            
            //if the query param is a date, filter by date
            if (DateTime.TryParse(query, out var date))
            {
                predicate.And(x => x.AttendanceDate.Date == date.Date);
            }
            
            var totalCount =
                
                await _repositoryStudentAttendance.CountAsync(predicate);
            
            var data = await (await _repositoryStudentAttendance.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.ClassName, a.ClassId, a.AcademicYearId, a.AcademicYearName, a.NoPresent, a.AttendanceDate, a.Details }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        
        public async Task<string> MarkBiometricAttendance(int tenantId, int biometricId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var encryptedBiometricId = _hashids.Encode(biometricId);
                var studentInfo = await _repositoryStudent.FirstOrDefaultAsync(x => x.BiometricIdentifier == encryptedBiometricId);

                if (studentInfo == null)
                {
                    return "Student not found.";
                }

                var schoolProfile = await (await _repositorySchoolProfile.GetAllAsync()).FirstOrDefaultAsync();
                var datta = await _repositoryStudentAttendance.FirstOrDefaultAsync(x => x.AttendanceDate.Date == DateTime.UtcNow.Date && x.ClassId == studentInfo.ClassId);

                if (datta != null)
                {
                    var alreadySignedIn = datta.Details.Any(d => d.StudentId == studentInfo.Id);
                    if (alreadySignedIn)
                    {
                        return $"Student {studentInfo.FirstName} {studentInfo.LastName} has already signed in for the day.";
                    }

                    datta.AcademicYearId = schoolProfile?.CurrentAcademicYearId ?? Guid.Empty;
                    datta.TermId = schoolProfile?.CurrentTermId ?? Guid.Empty;
                    datta.ClassId = studentInfo.ClassId;
                    datta.NoPresent += 1;
                    datta.AttendanceDate = DateTime.UtcNow;
                    datta.Details.Add(new StudentAttendanceDetail
                    {
                        Id = Guid.NewGuid(),
                        StudentId = studentInfo.Id,
                        StudentIdentifier = studentInfo.StudentIdentifier,
                        StudentName = string.IsNullOrEmpty(studentInfo.MiddleName) ? $"{studentInfo.FirstName} {studentInfo.LastName}" : $"{studentInfo.FirstName} {studentInfo.MiddleName} {studentInfo.LastName}"
                    });

                    await _repositoryStudentAttendance.UpdateAsync(datta);
                }
                else
                {
                    var studentAttendance = new StudentAttendance
                    {
                        ClassId = studentInfo.ClassId,
                        ClassName = studentInfo.ClassName,
                        AttendanceDate = DateTime.UtcNow,
                        NoPresent = 1,
                        AcademicYearId = schoolProfile.CurrentAcademicYearId,
                        AcademicYearName = schoolProfile.CurrentAcademicYearName,
                        TermId = schoolProfile.CurrentTermId,
                        TermName = schoolProfile.CurrentTermName,
                        Details = new List<StudentAttendanceDetail>
                        {
                            new()
                            {
                                Id = Guid.NewGuid(),
                                StudentId = studentInfo.Id,
                                StudentIdentifier = studentInfo.StudentIdentifier,
                                StudentName = string.IsNullOrEmpty(studentInfo.MiddleName) ? $"{studentInfo.FirstName} {studentInfo.LastName}" : $"{studentInfo.FirstName} {studentInfo.MiddleName} {studentInfo.LastName}"
                            }
                        },
                        TenantId = tenantId
                    };

                    await _repositoryStudentAttendance.InsertAsync(studentAttendance);
                }

                return $"Welcome {studentInfo.FirstName}.\nAttendance marked successfully.";
            }
        }

        
        //enquiry
        public async Task<object> ListAll(Guid classId, DateTime startDate, DateTime endDate)
        {
           return await _repositoryStudentAttendance.GetAllListAsync(x => x.ClassId == classId && x.AttendanceDate >= startDate && x.AttendanceDate <= endDate);
        }
    
}