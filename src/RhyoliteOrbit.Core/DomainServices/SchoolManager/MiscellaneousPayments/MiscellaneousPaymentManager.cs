using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.MiscellaneousPayments;

public class MiscellaneousPaymentManager: Abp.Domain.Services.DomainService, IMiscellaneousPaymentManager
{
    private readonly IRepository<MiscellaneousPayment, Guid> _repositoryMiscellaneousPayment;

        public MiscellaneousPaymentManager(IRepository<MiscellaneousPayment, Guid> repositoryMiscellaneousPayment)
        {
            _repositoryMiscellaneousPayment = repositoryMiscellaneousPayment;
        }

        public async Task Create(MiscellaneousPayment entity)
        {
            entity.ReceiptNo = DateTime.UtcNow.ToString("yyyyMMddhhmmss");
            await _repositoryMiscellaneousPayment.InsertAsync(entity);
        }


        public async Task<object> ListAll(DateTime startDate, DateTime endDate, Guid paymentTypeId)
        {
            var predicate = PredicateBuilder.New<MiscellaneousPayment>();

            predicate.And(x => !x.IsPosted);
            predicate.And(x => x.CreationTime >= startDate);
            predicate.And(x => x.CreationTime <= endDate.AddDays(1).AddTicks(-1));


            if (paymentTypeId != Guid.Empty)
            {
                predicate.And(x => x.PaymentTypeId == paymentTypeId);

            }

            return await _repositoryMiscellaneousPayment.GetAllListAsync(predicate);

        }

        public async Task<object> ListAll()
        {
            return await _repositoryMiscellaneousPayment.GetAllListAsync();
        }

        public async Task<object> ListAll(Guid paymentTypeId)
        {
            if (paymentTypeId == Guid.Empty)
            {
                return await _repositoryMiscellaneousPayment.GetAllListAsync();

            }

            return await _repositoryMiscellaneousPayment.GetAllListAsync(x=> x.PaymentTypeId == paymentTypeId);
        }

        public async Task<object> ListAllUnposted()
        {
            return await _repositoryMiscellaneousPayment.GetAllListAsync(x => !x.IsPosted);
        }
        
        public async Task BulkDelete(List<Guid> paymentList)
        {
            foreach (var paymentId in paymentList)
            {
                await _repositoryMiscellaneousPayment.DeleteAsync(paymentId);

            }

        }

        public async Task Delete(Guid id)
        {
            await _repositoryMiscellaneousPayment.DeleteAsync(id);
        }
    
    
}