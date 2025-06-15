using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using RhyoliteOrbit.DomainServices.Shared.MetaData;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.Siblings;

public class SiblingManager: Abp.Domain.Services.DomainService, ISiblingManager
{
        private readonly IRepository<Sibling,Guid> _repositorySibling;
        private readonly IRepository<Student,Guid> _repositoryStudent;
        private readonly IAcademicPeriodMetaDataManager _academicPeriodMetaDataManager;
        
        public SiblingManager(IRepository<Sibling, Guid> repositorySibling, IAcademicPeriodMetaDataManager academicPeriodMetaDataManager, IRepository<Student, Guid> repositoryStudent)
        {
            _repositorySibling = repositorySibling;
            _academicPeriodMetaDataManager = academicPeriodMetaDataManager;
            _repositoryStudent = repositoryStudent;
        }



        public async Task Create(Sibling entity)
        {
            var datta = await _repositorySibling.FirstOrDefaultAsync(x => x.SiblingId == entity.SiblingId);
            if (datta != null)
            {
                var siblingMeta = await _academicPeriodMetaDataManager.GetStudentMeta(entity.SiblingId, entity.SiblingClassId);
                var studentBasicInfo = await _repositoryStudent.FirstOrDefaultAsync(x => x.Id == entity.StudentId);

                datta.SiblingClassId = entity.SiblingClassId;
                datta.SiblingClass = siblingMeta.ClassName;
                datta.SiblingFullName = siblingMeta.StudentName;
                datta.SiblingIdentifier = siblingMeta.StudentIdentifier;
                datta.StudentId = entity.StudentId;

                entity.StudentIdentifier = studentBasicInfo.StudentIdentifier;
                entity.StudentName = string.IsNullOrEmpty(studentBasicInfo.MiddleName) ? $"{studentBasicInfo.FirstName} {studentBasicInfo.LastName}" : $"{studentBasicInfo.FirstName} {studentBasicInfo.MiddleName} {studentBasicInfo.LastName}";

                await _repositorySibling.UpdateAsync(datta);
            }
            else
            {
                var siblingMeta = await _academicPeriodMetaDataManager.GetStudentMeta(entity.SiblingId, entity.SiblingClassId);
                var studentBasicInfo = await _repositoryStudent.FirstOrDefaultAsync(x=>x.Id == entity.StudentId);

                entity.SiblingClass = siblingMeta.ClassName;
                entity.SiblingFullName = siblingMeta.StudentName;
                entity.SiblingIdentifier = siblingMeta.StudentIdentifier;

                entity.StudentIdentifier = studentBasicInfo.StudentIdentifier;
                entity.StudentName = string.IsNullOrEmpty(studentBasicInfo.MiddleName) ? $"{studentBasicInfo.FirstName} {studentBasicInfo.LastName}" : $"{studentBasicInfo.FirstName} {studentBasicInfo.MiddleName} {studentBasicInfo.LastName}";

                await _repositorySibling.InsertAsync(entity);
            }
        }


        public async Task<object> ListAll(Guid studentId)
        {
           return await _repositorySibling.GetAllListAsync(x=>x.StudentId == studentId);
        }

        public async Task Delete(Guid id)
        {
            await _repositorySibling.DeleteAsync(id);
        }

        public async Task Update(Sibling entity)
        {
            var siblingMeta = await _academicPeriodMetaDataManager.GetStudentMeta(entity.SiblingId, entity.SiblingClassId);
            var studentBasicInfo = await _repositoryStudent.FirstOrDefaultAsync(x => x.Id == entity.StudentId);

            entity.SiblingClassId = entity.SiblingClassId;
            entity.SiblingClass = siblingMeta.ClassName;
            entity.SiblingFullName = siblingMeta.StudentName;
            entity.SiblingIdentifier = siblingMeta.StudentIdentifier;
            entity.StudentId = entity.StudentId;

            entity.StudentIdentifier = studentBasicInfo.StudentIdentifier;
            entity.StudentName = string.IsNullOrEmpty(studentBasicInfo.MiddleName) ? $"{studentBasicInfo.FirstName} {studentBasicInfo.LastName}" : $"{studentBasicInfo.FirstName} {studentBasicInfo.MiddleName} {studentBasicInfo.LastName}";

            await _repositorySibling.UpdateAsync(entity);
        }
    
}