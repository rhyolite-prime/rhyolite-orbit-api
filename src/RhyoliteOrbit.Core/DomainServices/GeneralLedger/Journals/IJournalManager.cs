using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RhyoliteOrbit.DomainServices.GeneralLedger.Journals.Dto;

namespace RhyoliteOrbit.DomainServices.GeneralLedger.Journals;

public interface IJournalManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string batchNumber = null, DateTime? startDate = null, DateTime? endDate = null, bool? isPosted = null, bool? isMultiCurrency = null);

    Task Create(JournalDto dto);
    Task Update(JournalDto dto);
    Task<object> GetDetails(Guid id);
    Task BatchDelete(List<Guid> ids);
    Task Delete(Guid id);
}