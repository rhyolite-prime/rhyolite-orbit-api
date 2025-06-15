using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.PromotionLedger;

public class PromotionLedgerManager: Abp.Domain.Services.DomainService, IPromotionLedgerManager
{
    private readonly IRepository<PromotionHistory, Guid> _repositoryPromotionHistory;
        private readonly IRepository<SchoolProfile, Guid> _repositorySchoolProfile;
        private readonly IRepository<SchClass, Guid> _repositorySchClass;
        private readonly IRepository<Student, Guid> _repositoryStudent;

        public PromotionLedgerManager(IRepository<PromotionHistory, Guid> repositoryPromotionHistory, IRepository<SchoolProfile, Guid> repositorySchoolProfile, IRepository<SchClass, Guid> repositorySchClass, IRepository<Student, Guid> repositoryStudent)
        {
            _repositoryPromotionHistory = repositoryPromotionHistory;
            _repositorySchoolProfile = repositorySchoolProfile;
            _repositorySchClass = repositorySchClass;
            _repositoryStudent = repositoryStudent;
        }

        public async Task Create(PromotionHistory entity)
        {
            var schProfile = await (await _repositorySchoolProfile.GetAllAsync()).FirstOrDefaultAsync();
            if (schProfile != null)
            {
                var promotedFrom = await _repositorySchClass.FirstOrDefaultAsync(x=>x.Id == entity.PromotedFrom);
                var promotedTo = await _repositorySchClass.FirstOrDefaultAsync(x=>x.Id == entity.PromotedTo);
                var student = await _repositoryStudent.FirstOrDefaultAsync(x=>x.Id == entity.StudentId);

                entity.CurrentClassName = string.IsNullOrEmpty(promotedFrom.StreamName)? promotedFrom.ClassName : $"{promotedFrom.ClassName}-{promotedFrom.ClassName}";
                entity.DestinationClassName = string.IsNullOrEmpty(promotedTo.StreamName)? promotedTo.ClassName : $"{promotedTo.ClassName}-{promotedTo.ClassName}";
                entity.AcademicYearName = schProfile.CurrentAcademicYearName;
                entity.StudentName = string.IsNullOrEmpty(student.MiddleName)? $"{student.FirstName} {student.LastName}" : $"{student.FirstName} {student.MiddleName} {student.LastName}";
                entity.StudentIdentifier = student.StudentIdentifier ;
                entity.AcademicYearId = schProfile.CurrentAcademicYearId;
                await _repositoryPromotionHistory.InsertAsync(entity);

            }


        }

        public async Task<object> ListAll(Guid academicYearId)
        {
            return await _repositoryPromotionHistory.GetAllListAsync(x => x.AcademicYearId == academicYearId);
        }
    
    
}