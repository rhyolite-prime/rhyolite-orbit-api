using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using RhyoliteOrbit.DomainServices.Shared.MetaData;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.CancelledBills;

public class CancelledBillManager: Abp.Domain.Services.DomainService, ICancelledBillManager
{
    private readonly IRepository<CancelledBill, Guid> _repositoryCancelledBill;
    private readonly IAcademicPeriodMetaDataManager _academicPeriodMetaDataManager ;

    public CancelledBillManager(IRepository<CancelledBill, Guid> repositoryCancelledBill, IAcademicPeriodMetaDataManager academicPeriodMetaDataManager)
    {
        _repositoryCancelledBill = repositoryCancelledBill;
        _academicPeriodMetaDataManager = academicPeriodMetaDataManager;
    }

    public async Task CreateBatch(List<CancelledBill> bills)
    {

        foreach (CancelledBill entity in bills)
        {

            var metaData = await _academicPeriodMetaDataManager.GetAcademicPeriodMeta(entity.AcademicYearId, entity.TermId, entity.ClassId);
            entity.AcademicYearName = metaData.AcademicYearName;
            entity.TermName = metaData.TermName;
            entity.ClassName = metaData.ClassName;

            await _repositoryCancelledBill.InsertAsync(entity);

        }

    }

    public async Task<object> ListAll(Guid academicYearId, Guid termId, Guid classId, Guid billTypeId)
    {
        return await _repositoryCancelledBill.GetAllListAsync(x=> x.AcademicYearId == academicYearId && x.TermId == termId && x.ClassId == classId && x.BillTypeId == billTypeId);

    }
}