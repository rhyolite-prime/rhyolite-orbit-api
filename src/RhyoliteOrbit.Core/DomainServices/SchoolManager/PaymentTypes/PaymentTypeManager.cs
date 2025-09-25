using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.PaymentTypes;

public class PaymentTypeManager: Abp.Domain.Services.DomainService, IPaymentTypeManager
{
    private readonly IRepository<PaymentType, Guid> _repositoryPaymentType;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public PaymentTypeManager(IRepository<PaymentType, Guid> repositoryPaymentType, IUnitOfWorkManager unitOfWorkManager)
        {
            _repositoryPaymentType = repositoryPaymentType;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task Create(PaymentType entity)
        {
            var datta = await _repositoryPaymentType.FirstOrDefaultAsync(x => x.Name == entity.Name);
            
            if (datta == null)
            {
                await _repositoryPaymentType.InsertAsync(entity);

                return;
            }

            throw new UserFriendlyException(400, "Duplicate records are not allowed.");
            
        }

        public async Task CreateBulk(List<PaymentType> inputs, int tenantId)
        {

            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                foreach (var entity in inputs)
                {
                    entity.TenantId = tenantId;
                    await _repositoryPaymentType.InsertAsync(entity);

                }

            }

        }

        public async Task<object> GetAll(int pageNo, int pageSize, string query)
        {
            var predicate = PredicateBuilder.New<PaymentType>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.Name.Contains(query));

            }
            
            var totalCount =
                
                await _repositoryPaymentType.CountAsync(predicate);
            
            var data = await (await _repositoryPaymentType.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.TargetAudience }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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

       

        public async Task Delete(Guid id)
        {
            await _repositoryPaymentType.DeleteAsync(id);

        }

        public async Task Update(PaymentType entity)
        {
            await _repositoryPaymentType.UpdateAsync(entity);
        }
}