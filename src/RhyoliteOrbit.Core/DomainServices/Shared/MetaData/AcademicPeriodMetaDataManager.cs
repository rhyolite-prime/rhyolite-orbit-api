using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using RhyoliteOrbit.DomainServices.Shared.MetaData.Dto;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.Shared.MetaData;

public class AcademicPeriodMetaDataManager: DomainService, IAcademicPeriodMetaDataManager
{
        private readonly IRepository<AcademicYear, Guid> _repositoryAcademicYear;
        private readonly IRepository<SchClass, Guid> _repositorySchClass;
        private readonly IRepository<Student, Guid> _repositoryStudent;
        
        public AcademicPeriodMetaDataManager(IRepository<AcademicYear, Guid> repositoryAcademicYear, IRepository<SchClass, Guid> repositorySchClass, IRepository<Student, Guid> repositoryStudent)
        {
            _repositoryAcademicYear = repositoryAcademicYear;
            _repositorySchClass = repositorySchClass;
            _repositoryStudent = repositoryStudent;
        }

        public async Task<BasicAcademicInfo> GetAcademicPeriodMeta(Guid academicYearId,Guid termId,Guid classId)
        {
            var academicYearInfo = await _repositoryAcademicYear.FirstOrDefaultAsync(x => x.Id == academicYearId);

            var classInfo = await _repositorySchClass.FirstOrDefaultAsync(x => x.Id == classId);
            string className = string.Empty;

            if (classInfo != null && !string.IsNullOrEmpty(classInfo.StreamName))
            {
                className = $"{classInfo.ClassName}-{classInfo.StreamName}";
            }
            else if(classInfo != null && string.IsNullOrEmpty(classInfo.StreamName))
            {
                className = classInfo.ClassName;
            }

            return new BasicAcademicInfo
            {
                AcademicYearName = academicYearInfo == null ? "" : academicYearInfo.Name,
                ClassName = className,
                TermName = academicYearInfo != null && academicYearInfo.Terms.Any() ? academicYearInfo.Terms.FirstOrDefault(a => a.Id == termId)?.Name : "",
            };
        }

        public async Task<BasicAcademicInfo> GetAcademicPeriodMeta(Guid academicYearId, Guid termId)
        {
            var academicYearInfo = await _repositoryAcademicYear.FirstOrDefaultAsync(x => x.Id == academicYearId);
             

            return new BasicAcademicInfo
            {
                AcademicYearName = academicYearInfo == null ? "" : academicYearInfo.Name,
                TermName = academicYearInfo != null && academicYearInfo.Terms.Any() ? academicYearInfo.Terms.FirstOrDefault(a => a.Id == termId)?.Name : "",
            };
        }

        public async Task<BasicAcademicInfo> GetAcademicPeriodMeta(Guid academicYearId, Guid termId, Guid classId,Guid studentId)
        {
            var academicYearInfo = await _repositoryAcademicYear.FirstOrDefaultAsync(x => x.Id == academicYearId);
            
            var classInfo = await _repositorySchClass.FirstOrDefaultAsync(x => x.Id == classId);

            var studentInfo = await _repositoryStudent.FirstOrDefaultAsync(x => x.Id == studentId);


            string className = string.Empty;

            if (classInfo != null && !string.IsNullOrEmpty(classInfo.StreamName))
            {
                className = $"{classInfo.ClassName}-{classInfo.StreamName}";
            }
            else if (classInfo != null && string.IsNullOrEmpty(classInfo.StreamName))
            {
                className = classInfo.ClassName;
            }

            return new BasicAcademicInfo
            {
                AcademicYearName = academicYearInfo == null ? "": academicYearInfo.Name,
                ClassName = className,
                TermName = academicYearInfo != null && academicYearInfo.Terms.Any()? academicYearInfo.Terms.FirstOrDefault(a=>a.Id == termId)?.Name : "",
                StudentIdentifier = studentInfo.StudentIdentifier,
                StudentName = string.IsNullOrEmpty(studentInfo.MiddleName) ? $"{studentInfo.FirstName} {studentInfo.LastName}" : $"{studentInfo.FirstName} {studentInfo.MiddleName} {studentInfo.LastName}"

            };

        }

        public async Task<BasicAcademicInfo> GetStudentMeta(Guid studentId, Guid classId)
        {

            var classInfo = await _repositorySchClass.FirstOrDefaultAsync(x => x.Id == classId);

            var studentInfo = await _repositoryStudent.FirstOrDefaultAsync(x => x.Id == studentId);


            string className = string.Empty;

            if (classInfo != null && !string.IsNullOrEmpty(classInfo.StreamName))
            {
                className = $"{classInfo.ClassName}-{classInfo.StreamName}";
            }
            else if (classInfo != null && string.IsNullOrEmpty(classInfo.StreamName))
            {
                className = classInfo.ClassName;
            }

            return new BasicAcademicInfo
            {
                ClassName = className,
                StudentIdentifier = studentInfo.StudentIdentifier,
                StudentName = string.IsNullOrEmpty(studentInfo.MiddleName) ? $"{studentInfo.FirstName} {studentInfo.LastName}" : $"{studentInfo.FirstName} {studentInfo.MiddleName} {studentInfo.LastName}"
            };
        }
}