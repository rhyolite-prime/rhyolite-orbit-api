using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.SchClasses;

public class SchClassManager: Abp.Domain.Services.DomainService, ISchClassManager
{
        private readonly IRepository<SchClass, Guid> _repositorySchClass;
        private readonly IRepository<ClassStream, Guid> _repositoryClassStream;
        private readonly IRepository<Level, Guid> _repositoryLevel;

        public SchClassManager(IRepository<SchClass, Guid> repositorySchClass, IRepository<ClassStream, Guid> repositoryClassStream, IRepository<Level, Guid> repositoryLevel)
        {
            _repositorySchClass = repositorySchClass;
            _repositoryClassStream = repositoryClassStream;
            _repositoryLevel = repositoryLevel;
        }


        public async Task Delete(Guid id)
        {
            await _repositorySchClass.DeleteAsync(id);
        } 

        public async Task<object> Create(SchClass entity)
        {

            var schClass = await _repositorySchClass.FirstOrDefaultAsync(x => x.LevelId == entity.LevelId && x.ClassName == entity.ClassName && x.StreamId == entity.StreamId);
            if (schClass == null)
            {
                var stream = await _repositoryClassStream.FirstOrDefaultAsync(x => x.Id == entity.StreamId);
                var level = await _repositoryLevel.FirstOrDefaultAsync(x => x.Id == entity.LevelId);


                if (level != null)
                {
                    entity.LevelName = level.Name;
                }

                if (stream != null)
                {
                    entity.StreamName = stream.Name;
                }

                await _repositorySchClass.InsertAsync(entity);

                return new
                {
                    code = 200,
                    message = "successful"
                };
            }
            
            throw new UserFriendlyException(400, "Duplicate records are not allowed.");

        }


        public async Task<object> GetAsync(Guid id)
        {
            return await _repositorySchClass.GetAsync(id);
        }


        public async Task<object> ListAll(Guid levelId)
        {
            var data = await _repositorySchClass.GetAllListAsync(a => a.LevelId == levelId);
            
            return data.OrderBy(c => c.ClassName);
        }

        public async Task<object> ListAll(int pageNo, int pageSize, string query)
        {
            var predicate = PredicateBuilder.New<SchClass>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.ClassName.ToLower().Contains(query.ToLower()) || x.LevelName.ToLower().Contains(query.ToLower()) || x.StreamName.ToLower().Contains(query.ToLower()));
            }
            
            var totalCount =
                
                await _repositorySchClass.CountAsync(predicate);
            
            var data = await (await _repositorySchClass.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.ClassName, a.LevelName, a.StreamName, a.LevelId, a.StreamId }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

            var totalPages = totalCount > 0 ? Convert.ToInt32(Math.Ceiling(totalCount / (decimal)pageSize)) : 0;

            var paginatedResult = new
            {
                pageNo,
                pageSize,
                totalCount,
                totalPages,
                data,
                lowerBound = pageSize * (pageNo - 1) + 1,
                upperBound = totalPages == pageNo ? totalCount : pageNo * pageSize,
            };

            return paginatedResult;
        }

        public async Task Update(SchClass entity)
        {
            var stream = await _repositoryClassStream.FirstOrDefaultAsync(x => x.Id == entity.StreamId);
            var level = await _repositoryLevel.FirstOrDefaultAsync(x => x.Id == entity.LevelId);

            entity.LevelName = level.Name;
            if (stream != null)
            {
                entity.StreamName = stream.Name;
            }

            await _repositorySchClass.UpdateAsync(entity);
        }
}