using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.ResultTypes;

public class ResultTypeManager: Abp.Domain.Services.DomainService, IResultTypeManager
{
    private readonly IRepository<ResultType, Guid> _repositoryResultType;
        private readonly IRepository<Level, Guid> _repositoryLevel;
        private readonly IRepository<SchClass, Guid> _repositorySchClass;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public ResultTypeManager(IRepository<ResultType, Guid> repositoryResultType, IUnitOfWorkManager unitOfWorkManager, IRepository<Level, Guid> repositoryLevel, IRepository<SchClass, Guid> repositorySchClass)
        {
            _repositoryResultType = repositoryResultType;
            _unitOfWorkManager = unitOfWorkManager;
            _repositoryLevel = repositoryLevel;
            _repositorySchClass = repositorySchClass;
        }

        public async Task<object> Create(ResultType entity)
        {

            var level = await _repositoryLevel.FirstOrDefaultAsync(x=>x.Id == entity.LevelId);

            var datta = await _repositoryResultType.FirstOrDefaultAsync(x => x.Name == entity.Name && x.LevelId == entity.LevelId);
            if (datta == null)
            {
                if (level !=null)
                {
                    entity.LevelName = level.Name;
                }
                await _repositoryResultType.InsertAsync(entity);
                return new
                {
                    code = 200,
                    message = "successful"
                };

            }

            throw new UserFriendlyException(400, "Duplicate records are not allowed.");
            
        }

        public async Task Update(ResultType entity)
        {

            var level = await _repositoryLevel.FirstOrDefaultAsync(x => x.Id == entity.LevelId);
            if (level !=null)
            {
                entity.LevelName = level.Name;
            }
            await _repositoryResultType.UpdateAsync(entity);
        }


        public async Task<object> ListAll(Guid levelId)
        {
            return await _repositoryResultType.GetAllListAsync(x => x.LevelId == levelId);
        }

        public async Task<object> ListAll(int tenantId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            { 
                return await _repositoryResultType.GetAllListAsync();
            }
        }

        public async Task<object> ListByClass(Guid classId)
        {
            var schClass = await _repositorySchClass.FirstOrDefaultAsync(x => x.Id == classId);

            return await _repositoryResultType.GetAllListAsync(x => x.LevelId == schClass.LevelId);
        }

       

        public async Task CreateBulk(List<ResultType> inputs, int tenantId)
        {

            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                foreach (var entity in inputs)
                {
                    var level = await _repositoryLevel.FirstOrDefaultAsync(x => x.Id == entity.Id);
                    entity.LevelName = level.Name;
                    entity.TenantId = tenantId;

                    await _repositoryResultType.InsertAsync(entity);

                }

            }

        }

        public async Task Delete(Guid id)
        {
            await _repositoryResultType.DeleteAsync(id);
        }
}