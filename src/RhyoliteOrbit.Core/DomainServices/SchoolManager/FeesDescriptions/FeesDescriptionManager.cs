using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.UI;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.FeesDescriptions;

public class FeesDescriptionManager: Abp.Domain.Services.DomainService, IFeesDescriptionManager
{
    private readonly IRepository<FeesDescription, Guid> _repositoryFeesDescription;
        private readonly IRepository<BillType, Guid> _repositoryBillType;

        public FeesDescriptionManager(IRepository<FeesDescription, Guid> repositoryFeesDescription, IRepository<BillType, Guid> repositoryBillType)
        {
            _repositoryFeesDescription = repositoryFeesDescription;
            _repositoryBillType = repositoryBillType;
        }

        public async Task<object> Create(FeesDescription entity)
        {
            var datta = await _repositoryFeesDescription.FirstOrDefaultAsync(x => x.Description == entity.Description && x.BillTypeId == entity.BillTypeId);
            if (datta == null)
            {
                var billTypeInfo = await _repositoryBillType.FirstOrDefaultAsync(x=>x.Id == entity.BillTypeId);

                if (billTypeInfo != null)
                {
                    entity.BillTypeName = billTypeInfo.Name;
                }

                await _repositoryFeesDescription.InsertAsync(entity);

                return new
                {
                    code = 200,
                    message = "successful"
                };
            }

            throw new UserFriendlyException(400, "Duplicate records are not allowed.");
            
        }

        public async Task<object> ListAll()
        {
            return await _repositoryFeesDescription.GetAllListAsync();
        }
        public async Task<object> ListAll(Guid billTypeId)
        {
            return await _repositoryFeesDescription.GetAllListAsync(a => a.BillTypeId == billTypeId);
        }
        public async Task Delete(Guid id)
        {
            await _repositoryFeesDescription.DeleteAsync(id);
        }

        public async Task Update(FeesDescription entity)
        {
            await _repositoryFeesDescription.UpdateAsync(entity);
        }
}