using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using RhyoliteOrbit.DomainServices.Shared.MetaData;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.SchoolManager.CancelledPayments;

public class CancelledPaymentManager
{
    private readonly IRepository<CancelledPayment, Guid> _repositoryCancelledPayment;
        private readonly IRepository<Currency, Guid> _repositoryCurrency;

        private readonly IAcademicPeriodMetaDataManager _academicPeriodMetaDataManager;

        public CancelledPaymentManager(IRepository<CancelledPayment, Guid> repositoryCancelledPayment, IAcademicPeriodMetaDataManager academicPeriodMetaDataManager, IRepository<Currency, Guid> repositoryCurrency)
        {
            _repositoryCancelledPayment = repositoryCancelledPayment;
            _academicPeriodMetaDataManager = academicPeriodMetaDataManager;
            _repositoryCurrency = repositoryCurrency;
        }

        public async Task CreateBatch(List<CancelledPayment> payments)
        {
            foreach (CancelledPayment entity in payments)
            {
                var metaData = await _academicPeriodMetaDataManager.GetAcademicPeriodMeta(entity.AcademicYearId, entity.TermId, entity.ClassId, entity.StudentId);
                var currency = await _repositoryCurrency.FirstOrDefaultAsync(x=>x.Id == entity.CurrencyId);

                var cancelledPayment = new CancelledPayment
                {
                    AcademicYearId = entity.AcademicYearId,
                    AcademicYearName = metaData.AcademicYearName,
                    TermName = metaData.TermName,
                    ClassName = metaData.ClassName,
                    TermId = entity.TermId,
                    ClassId = entity.ClassId,
                    StudentId = entity.StudentId,
                    StudentIdentifier = metaData.StudentIdentifier,
                    StudentName = metaData.StudentName,
                    AmountPaid = entity.AmountPaid,
                    ModeOfPayment = entity.ModeOfPayment,
                    PaymentDate = entity.PaymentDate,
                    CurrencyId = entity.CurrencyId,
                    CurrencyBuyRate = currency.BuyRate,
                    CurrencySellRate = currency.SellRate,
                    ChequeNo = entity.ChequeNo,
                    BillId = entity.BillId,
                    BillNo = entity.BillNo,
                    ReceiptNo = entity.ReceiptNo,
                    PaymentDescription = entity.PaymentDescription,
                    IsCreditMemo = entity.IsCreditMemo,
                };

                await _repositoryCancelledPayment.InsertAsync(cancelledPayment);
            }

        }

        public async Task<object> ListAll(Guid academicYearId, Guid termId, Guid classId)
        {
            return await _repositoryCancelledPayment.GetAllListAsync(x => x.AcademicYearId == academicYearId && x.TermId == termId && x.ClassId == classId);
        }
}