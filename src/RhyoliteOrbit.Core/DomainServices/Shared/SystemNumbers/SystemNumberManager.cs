using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.SystemNumbers;

public class SystemNumberManager: DomainService, ISystemNumberManager
{
        private readonly IRepository<SystemNumber, Guid> _repositorySystemNumber;

        public SystemNumberManager(IRepository<SystemNumber, Guid> repositorySystemNumber)
        {
                _repositorySystemNumber = repositorySystemNumber;
        }


        public async Task Create(List<SystemNumber> systemNumbers)
        {
                foreach (var systemNumber in systemNumbers)
                {
                        await _repositorySystemNumber.InsertAsync(systemNumber);
                }
        }
}