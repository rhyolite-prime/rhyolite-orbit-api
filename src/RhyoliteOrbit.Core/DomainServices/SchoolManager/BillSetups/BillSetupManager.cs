using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.DomainServices.Shared.MetaData;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.BillSetups;

public class BillSetupManager: Abp.Domain.Services.DomainService, IBillSetupManager
{
    private readonly IRepository<BillSetup, Guid> _repositoryBillSetup;
        private readonly IRepository<BillType, Guid> _repositoryBillType;
        private readonly IRepository<FeesDescription, Guid> _repositoryFeesDescription;
        private readonly IAcademicPeriodMetaDataManager _academicPeriodMetaDataManager;

        public BillSetupManager(IRepository<BillSetup, Guid> repositoryBillSetup, IAcademicPeriodMetaDataManager academicPeriodMetaDataManager, IRepository<BillType, Guid> repositoryBillType, IRepository<FeesDescription, Guid> repositoryFeesDescription)
        {
            _repositoryBillSetup = repositoryBillSetup;
            _academicPeriodMetaDataManager = academicPeriodMetaDataManager;
            _repositoryBillType = repositoryBillType;
            _repositoryFeesDescription = repositoryFeesDescription;
        }


        public async Task<object> Create(BillSetup entity)
        {
            // Validate input
            if (entity == null)
            {
                throw new UserFriendlyException(400, "Argument Exception");
            }

            if (entity.Details == null || !entity.Details.Any())
            {
                throw new UserFriendlyException(400, "Argument Exception", "Bill details cannot be empty");
            }

            // Get related data
            var billTypeInfo = await _repositoryBillType.FirstOrDefaultAsync(x => x.Id == entity.BillTypeId);
            var basicInfo = await _academicPeriodMetaDataManager.GetAcademicPeriodMeta(entity.AcademicYearId, entity.TermId, entity.ClassId);
            var existingBillSetup = await _repositoryBillSetup.FirstOrDefaultAsync(x => 
                x.AcademicYearId == entity.AcademicYearId && 
                x.TermId == entity.TermId && 
                x.ClassId == entity.ClassId && 
                x.BillTypeId == entity.BillTypeId);

            // Process first bill detail
            var billDetail = entity.Details.FirstOrDefault();
            var feeDescriptionInfo = await _repositoryFeesDescription.FirstOrDefaultAsync(x => x.Id == billDetail.FeeId);
            
            // Create the bill detail object
            var newBillDetail = CreateBillDetail(billDetail, feeDescriptionInfo, entity.BillTypeId, billTypeInfo?.Name);

            if (existingBillSetup != null)
            {
                return await UpdateExistingBillSetup(existingBillSetup, entity, basicInfo, billTypeInfo, newBillDetail);
            }
            else
            {
                return await CreateNewBillSetup(entity, basicInfo, billTypeInfo, newBillDetail);
            }
        }

        private BillSetupDetail CreateBillDetail(BillSetupDetail sourceDetail, FeesDescription feeDescriptionInfo, Guid billTypeId, string billTypeName)
        {
            return new BillSetupDetail
            {
                Id = sourceDetail.Id,
                FeeAmount = sourceDetail.FeeAmount,
                FeeId = sourceDetail.FeeId,
                FeeName = feeDescriptionInfo?.Description,
                BillTypeId = billTypeId,
                BillTypeName = billTypeName ?? string.Empty
            };
        }

        private async Task<object> UpdateExistingBillSetup(
            BillSetup existingBillSetup, 
            BillSetup newEntity, 
            dynamic basicInfo, 
            BillType billTypeInfo, 
            BillSetupDetail newBillDetail)
        {
            // Update basic information
            existingBillSetup.AcademicYearName = basicInfo.AcademicYearName;
            existingBillSetup.TermName = basicInfo.TermName;
            existingBillSetup.ClassName = basicInfo.ClassName;
            existingBillSetup.BillTypeId = newEntity.BillTypeId;
            existingBillSetup.BillTypeName = billTypeInfo?.Name;
            existingBillSetup.TotalBillAmount = newEntity.TotalBillAmount;

            // Update bill details
            var billDetails = existingBillSetup.Details;
            var existingBillDetail = billDetails.FirstOrDefault(x => x.FeeId == newEntity.Details[0].FeeId);
            
            if (existingBillDetail != null)
            {
                billDetails.Remove(existingBillDetail);
            }

            billDetails.Add(newBillDetail);
            existingBillSetup.Details = billDetails;

            return await _repositoryBillSetup.UpdateAsync(existingBillSetup);
        }

        private async Task<object> CreateNewBillSetup(
            BillSetup entity, 
            dynamic basicInfo, 
            BillType billTypeInfo, 
            BillSetupDetail billDetail)
        {
            var newBillSetup = new BillSetup
            {
                AcademicYearId = entity.AcademicYearId,
                TermId = entity.TermId,
                ClassId = entity.ClassId,
                ClassName = basicInfo.ClassName,
                TermName = basicInfo.TermName,
                AcademicYearName = basicInfo.AcademicYearName,
                BillTypeId = entity.BillTypeId,
                BillTypeName = billTypeInfo?.Name,
                TotalBillAmount = entity.TotalBillAmount,
                Details = new List<BillSetupDetail> { billDetail },
                TenantId = entity.TenantId
            };

            return await _repositoryBillSetup.InsertAsync(newBillSetup);
        }

        public async Task<object> ListAll(int pageNo, int pageSize, string query, Guid academicYearId, Guid termId, Guid classId, Guid billTypeId)
        {
            var predicate = PredicateBuilder.New<BillSetup>();
        
            predicate.And(x => !x.IsDeleted);
        
            if (academicYearId != Guid.Empty)
                predicate.And(x => x.AcademicYearId == academicYearId);
        
            if (termId != Guid.Empty)
                predicate.And(x => x.TermId == termId);
        
            if (classId != Guid.Empty)
                predicate.And(x => x.ClassId == classId);
        
            if (billTypeId != Guid.Empty)
                predicate.And(x => x.BillTypeId == billTypeId);
        
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.AcademicYearName.ToLower().Contains(query.ToLower()) ||
                                   x.TermName.ToLower().Contains(query.ToLower()) ||
                                   x.ClassName.ToLower().Contains(query.ToLower()) ||
                                   x.BillTypeName.ToLower().Contains(query.ToLower()));
            }
        
            var totalCount = await _repositoryBillSetup.CountAsync(predicate);
        
            var data = await (await _repositoryBillSetup.GetAllAsync())
                .Where(predicate)
                .Select(a => new
                {
                    a.Id,
                    a.AcademicYearId,
                    a.AcademicYearName,
                    a.TermId,
                    a.TermName,
                    a.ClassId,
                    a.ClassName,
                    a.BillTypeId,
                    a.BillTypeName,
                    a.TotalBillAmount,
                    a.Details
                })
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        
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

        public async Task<object> GetAsync(Guid academicYearId, Guid termId, Guid classId, Guid billTypeId)
        {
            return await _repositoryBillSetup.FirstOrDefaultAsync(x => x.AcademicYearId == academicYearId && x.TermId == termId && x.ClassId == classId && x.BillTypeId == billTypeId);
        }

        public async Task Delete(Guid id)
        {
            await _repositoryBillSetup.DeleteAsync(id);
        }

        public async Task DeleteDetail(Guid id, Guid headerId)
        {
            var billSetup = await _repositoryBillSetup.GetAsync(headerId);

            var detailInfo =  billSetup.Details.FirstOrDefault(a=>a.Id == id);

            billSetup.Details.Remove(detailInfo);

            await _repositoryBillSetup.UpdateAsync(billSetup);
        }
    }
