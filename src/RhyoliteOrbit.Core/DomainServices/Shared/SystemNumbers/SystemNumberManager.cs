using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using RhyoliteOrbit.DomainServices.Shared.Banks.Dto;
using RhyoliteOrbit.Models.GeneralLedger;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.SystemNumbers;

public class SystemNumberManager: DomainService, ISystemNumberManager
{
        private readonly IRepository<SystemNumber, Guid> _repositorySystemNumber;

        public SystemNumberManager(IRepository<SystemNumber, Guid> repositorySystemNumber)
        {
                _repositorySystemNumber = repositorySystemNumber;
        }

        public async Task<object> ListAll(string key)
        {
                var predicate = PredicateBuilder.New<SystemNumber>();

                predicate.And(x => !x.IsDeleted);
                if (!string.IsNullOrEmpty(key))
                {
                     predicate.And(x => x.Key.ToLower().Contains(key.ToLower()));
                }

                return await _repositorySystemNumber.GetAllListAsync(predicate);
        }
        
        public async Task Update(SystemNumber entity)
        {
          await _repositorySystemNumber.UpdateAsync(entity);
        }
        
        public async Task Create(List<SystemNumber> systemNumbers)
        {
                foreach (var systemNumber in systemNumbers)
                {
                        await _repositorySystemNumber.InsertAsync(systemNumber);
                }
        }
        
        
}