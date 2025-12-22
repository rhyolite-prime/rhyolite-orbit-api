using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Expressions;
using Abp.UI;
using HashidsNet;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.DomainServices.Shared.MetaData;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.BillSetups;

public class BillSetupManager: Abp.Domain.Services.DomainService, IBillSetupManager
{
        private readonly IRepository<BillSetup, Guid> _repositoryBillSetup;
        private readonly IRepository<BillType, Guid> _repositoryBillType;
        private readonly IRepository<FeesDescription, Guid> _repositoryFeesDescription;
        private readonly IRepository<Student, Guid> _repositoryStudent;
        private readonly IAcademicPeriodMetaDataManager _academicPeriodMetaDataManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IHashids _hashids;

        public BillSetupManager(IRepository<BillSetup, Guid> repositoryBillSetup, IAcademicPeriodMetaDataManager academicPeriodMetaDataManager, IRepository<BillType, Guid> repositoryBillType, IRepository<FeesDescription, Guid> repositoryFeesDescription, IUnitOfWorkManager unitOfWorkManager, IRepository<Student, Guid> repositoryStudent, IHashids hashids)
        {
            _repositoryBillSetup = repositoryBillSetup;
            _academicPeriodMetaDataManager = academicPeriodMetaDataManager;
            _repositoryBillType = repositoryBillType;
            _repositoryFeesDescription = repositoryFeesDescription;
            _unitOfWorkManager = unitOfWorkManager;
            _repositoryStudent = repositoryStudent;
            _hashids = hashids;
        }


        
        public async Task<object> Create(BillSetup entity)
        {
            // Validate input
            if (entity == null)
                throw new UserFriendlyException(400, "Argument Exception");

            if (entity.Details == null || !entity.Details.Any())
                throw new UserFriendlyException(400, "Argument Exception", "Bill details cannot be empty");

            // Get related data
            var billTypeInfo = await _repositoryBillType.FirstOrDefaultAsync(x => x.Id == entity.BillTypeId);
            var basicInfo = await _academicPeriodMetaDataManager.GetAcademicPeriodMeta(entity.AcademicYearId, entity.TermId, entity.ClassId);

            var existingBillSetup = await _repositoryBillSetup.FirstOrDefaultAsync(x =>
                x.AcademicYearId == entity.AcademicYearId &&
                x.TermId == entity.TermId &&
                x.ClassId == entity.ClassId &&
                x.BillTypeId == entity.BillTypeId);

            // Batch fetch fee descriptions for all details
            var feeIds = entity.Details
                .Where(d => d.FeeId != Guid.Empty)
                .Select(d => d.FeeId)
                .Distinct()
                .ToList();

            var feeDescriptions = feeIds.Any()
                ? await _repositoryFeesDescription.GetAllListAsync(fd => feeIds.Contains(fd.Id))
                : new List<FeesDescription>();

            // Build BillSetupDetail objects for each incoming detail
            var newBillDetails = entity.Details.Select(d =>
            {
                var feeDesc = feeDescriptions.FirstOrDefault(f => f.Id == d.FeeId);
                return CreateBillDetail(d, feeDesc, entity.BillTypeId, billTypeInfo?.Name);
            }).ToList();

            if (existingBillSetup != null)
            {
                return await UpdateExistingBillSetup(existingBillSetup, entity, basicInfo, billTypeInfo, newBillDetails);
            }
            else
            {
                return await CreateNewBillSetup(entity, basicInfo, billTypeInfo, newBillDetails);
            }
        }

        private BillSetupDetail CreateBillDetail(BillSetupDetail sourceDetail, FeesDescription feeDescriptionInfo, Guid billTypeId, string billTypeName)
        {
            // preserve Id if provided; otherwise repository can generate
            return new BillSetupDetail
            {
                Id = sourceDetail.Id,
                FeeAmount = sourceDetail.FeeAmount,
                FeeId = sourceDetail.FeeId,
                FeeName = feeDescriptionInfo?.Description,
                BillTypeId = billTypeId,
                Gender = string.IsNullOrWhiteSpace(sourceDetail.Gender) ? string.Empty : sourceDetail.Gender,
                BillTypeName = billTypeName ?? string.Empty
            };
        }

        private async Task<object> UpdateExistingBillSetup(
            BillSetup existingBillSetup,
            BillSetup newEntity,
            dynamic basicInfo,
            BillType billTypeInfo,
            List<BillSetupDetail> newBillDetails)
        {
            // Update basic information
            existingBillSetup.AcademicYearName = basicInfo.AcademicYearName;
            existingBillSetup.TermName = basicInfo.TermName;
            existingBillSetup.ClassName = basicInfo.ClassName;
            existingBillSetup.BillTypeId = newEntity.BillTypeId;
            existingBillSetup.BillTypeName = billTypeInfo?.Name;
            existingBillSetup.TotalBillAmount = newEntity.TotalBillAmount;

            // Ensure existing details collection exists
            var billDetails = existingBillSetup.Details ?? new List<BillSetupDetail>();

            // Merge incoming details:
            // - Remove an existing detail if it matches FeeId + Gender (case-insensitive)
            // - Add the incoming detail
            foreach (var incoming in newBillDetails)
            {
                var existingDetail = billDetails.FirstOrDefault(x =>
                    x.FeeId == incoming.FeeId &&
                    string.Equals((x.Gender ?? ""), (incoming.Gender ?? ""), StringComparison.OrdinalIgnoreCase));

                if (existingDetail != null)
                    billDetails.Remove(existingDetail);

                billDetails.Add(incoming);
            }

            existingBillSetup.Details = billDetails;

            return await _repositoryBillSetup.UpdateAsync(existingBillSetup);
        }

        private async Task<object> CreateNewBillSetup(
            BillSetup entity,
            dynamic basicInfo,
            BillType billTypeInfo,
            List<BillSetupDetail> billDetails)
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
                Details = billDetails ?? new List<BillSetupDetail>(),
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
