using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.Domain.Services;
using Abp.UI;
using RhyoliteOrbit.DomainServices.GeneralLedger.Journals.Dto;
using RhyoliteOrbit.Models.GeneralLedger;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.GeneralLedger.Journals;


public class JournalManager: DomainService, IJournalManager
{
    private readonly IRepository<Journal, Guid> _repositoryJournal;
    private readonly IRepository<JournalDetail, Guid> _repositoryJournalDetail;
    private readonly IRepository<SystemNumber, Guid> _repositorySystemNumber;

    public JournalManager(IRepository<Journal, Guid> repositoryJournal, IRepository<JournalDetail, Guid> repositoryJournalDetail, IRepository<SystemNumber, Guid> repositorySystemNumber)
    {
        _repositoryJournal = repositoryJournal;
        _repositoryJournalDetail = repositoryJournalDetail;
        _repositorySystemNumber = repositorySystemNumber;
    }

    
    public async Task Create(JournalDto dto)
    {
        var journalId = Guid.NewGuid();
        
        if (dto.BatchDate == default)
        {
            dto.BatchDate = DateTime.UtcNow;
        }
        
        await _repositoryJournal.InsertAsync(new Journal
        {
            BatchDate = dto.BatchDate,
            BatchNumber = dto.BatchNumber, 
            Id = journalId, 
            OuId = dto.OuId,
            OuName = dto.OuName,
            IsPosted = false,
            IsMultiCurrency = dto.IsMultiCurrency, 
            TotalCredit = dto.TotalCredit, 
            TotalDebit = dto.TotalDebit,
        });

        // insert details
        foreach (var detailDto in dto.Details)
        {
            detailDto.JournalId = journalId;
            await _repositoryJournalDetail.InsertAsync(detailDto);
        }
        
        //increase the next batch number in system numbers
        var journalBatchSystemNumber = await _repositorySystemNumber.FirstOrDefaultAsync(a=> a.Key == "journal-batch-id");

        if (journalBatchSystemNumber != null)
        {
            journalBatchSystemNumber.LastNo += 1;
        }
            
    }
        
    public async Task Update(JournalDto dto)
    {
        // 1. Fetch the existing journal batch header by BatchNumber (or by Id depending on your DTO)
        var journal = await _repositoryJournal.FirstOrDefaultAsync(j => j.BatchNumber.Trim() == dto.BatchNumber.Trim());

        if (journal == null)
        {
            throw new UserFriendlyException($"Journal batch with batch number '{dto.BatchNumber}' was not found.");
        }

        // Optional Guard: Prevent updating if the journal has already been posted
        if (journal.IsPosted)
        {
            throw new UserFriendlyException("Cannot modify a journal batch that has already been posted.");
        }

        // 2. Update journal header properties
        journal.BatchDate = dto.BatchDate == default ? DateTime.UtcNow : dto.BatchDate;
        journal.IsMultiCurrency = dto.IsMultiCurrency;
        journal.TotalCredit = dto.TotalCredit;
        journal.TotalDebit = dto.TotalDebit;
        journal.OuId = dto.OuId;
        journal.OuName = dto.OuName;

        await _repositoryJournal.UpdateAsync(journal);

        // 3. Delete existing journal details for this batch
        await _repositoryJournalDetail.HardDeleteAsync(d => d.JournalId == journal.Id);

        // 4. Insert updated line items
        if (dto.Details != null && dto.Details.Any())
        {
            foreach (var detailDto in dto.Details)
            {
                detailDto.JournalId = journal.Id;
                await _repositoryJournalDetail.InsertAsync(detailDto);
            }
        }
    }
    public async Task<object> ListAll(int pageNo, int pageSize, string batchNumber = null, DateTime? startDate = null, DateTime? endDate = null, bool? isPosted = null, bool? isMultiCurrency = null)
    {
        var predicate = PredicateBuilder.New<Journal>();
        
        predicate.And(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(batchNumber))
        {
            predicate.And(x => x.BatchNumber.Trim() == batchNumber.Trim());
        }

        if (startDate.HasValue)
        {
            predicate.And(x => x.BatchDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            predicate.And(x => x.BatchDate <= endDate.Value);
        }

        if (isPosted.HasValue)
        {
            predicate.And(x => x.IsPosted == isPosted.Value);
        }

        if (isMultiCurrency.HasValue)
        {
            predicate.And(x => x.IsMultiCurrency == isMultiCurrency.Value);
        }

        var totalCount = await _repositoryJournal.CountAsync(predicate);

        var journals = await (await _repositoryJournal.GetAllAsync()).Where(predicate).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
        
        var journalIds = journals.Select(j => j.Id).ToList();

        var details = await _repositoryJournalDetail.GetAllListAsync(d => journalIds.Contains(d.JournalId));

        var data = journals.Select(j => new JournalDto
        {
            BatchNumber = j.BatchNumber,
            BatchDate = j.BatchDate,
            TotalCredit = j.TotalCredit,
            TotalDebit = j.TotalDebit,
            OuId = j.OuId,
            Id =  j.Id,
            OuName = j.OuName,
            IsMultiCurrency = j.IsMultiCurrency,
            Details = details.Where(d => d.JournalId == j.Id).ToList()
        }).ToList();

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

    public async Task<object> GetDetails(Guid id)
    {
        return await _repositoryJournal.FirstOrDefaultAsync(id);
    }
    
    public async Task BatchDelete(List<Guid> ids)
    {
        foreach (var t in ids)
        {
            await _repositoryJournal.DeleteAsync(t);
        }
            
    }
    
    public async Task Delete(Guid id)
    {
        await _repositoryJournal.DeleteAsync(id);
        
        var journalDetails = await _repositoryJournalDetail.GetAllListAsync(x=> x.JournalId == id);
        
        foreach (var journalDetail in journalDetails)
        {
            await _repositoryJournalDetail.DeleteAsync(journalDetail);
        }
    }
}