using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Expressions;
using Akka.Actor;
using AutoMapper;
using HashidsNet;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Actors;
using RhyoliteOrbit.Actors.MessageModels;
using RhyoliteOrbit.DomainServices.SchoolManager.StudentBills.Dto;
using RhyoliteOrbit.DomainServices.Shared.MetaData;
using RhyoliteOrbit.DomainServices.Shared.SystemSettings;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.SchoolManager.StudentBills;

public class StudentBillManager: Abp.Domain.Services.DomainService, IStudentBillManager
{
        private readonly IRepository<StudentBill, Guid> _repositoryStudentBill;
        private readonly IRepository<CancelledBill, Guid> _repositoryCancelledBill;
        private readonly IRepository<BillSetup, Guid> _repositoryBillSetup;
        private readonly IRepository<BillType, Guid> _repositoryBillType;
        private readonly IRepository<StudentStatement, Guid> _repositoryStudentStatement;
        private readonly IRepository<StudentBillReportHistory, Guid> _repositoryStudentBillReportHistory;
        private readonly IRepository<Currency, Guid> _repositoryCurrency;
        private readonly IRepository<Student, Guid> _repositoryStudent;
        private readonly IRepository<Parent, Guid> _repositoryParent;
        private readonly IRepository<StudentParent, Guid> _repositoryStudentParent;
        private readonly IRepository<SystemSetting, Guid> _repositorySystemSetting;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IAcademicPeriodMetaDataManager _academicPeriodMetaDataManager;
        private readonly ISystemSettingManager _systemSettingManager;
        private readonly IActorRef _studentBillActorProvider;
        private readonly IHashids _hashids;
        private readonly IMapper _mapper;
        

        public StudentBillManager(IRepository<StudentBill, Guid> repositoryStudentBill, TopLevelActors.StudentBillActorProvider studentBillActorProvider, IRepository<StudentStatement, Guid> repositoryStudentStatement, IRepository<Currency, Guid> repositoryCurrency, IRepository<SystemSetting, Guid> repositorySystemSetting, IRepository<BillSetup, Guid> repositoryBillSetup, IAcademicPeriodMetaDataManager academicPeriodMetaDataManager, IRepository<BillType, Guid> repositoryBillType, IRepository<CancelledBill, Guid> repositoryCancelledBill, IMapper mapper, IRepository<Student, Guid> repositoryStudent, IUnitOfWorkManager unitOfWorkManager, IRepository<Parent, Guid> repositoryParent, IRepository<StudentParent, Guid> repositoryStudentParent, ISystemSettingManager systemSettingManager, IHashids hashids, IRepository<StudentBillReportHistory, Guid> repositoryStudentBillReportHistory)
        {
            _repositoryStudentBill = repositoryStudentBill;
            _repositoryStudentStatement = repositoryStudentStatement;
            _repositoryCurrency = repositoryCurrency;
            _repositorySystemSetting = repositorySystemSetting;
            _repositoryBillSetup = repositoryBillSetup;
            _repositoryBillType = repositoryBillType;
            _repositoryCancelledBill = repositoryCancelledBill;
            _mapper = mapper;
            _repositoryStudent = repositoryStudent;
            _unitOfWorkManager = unitOfWorkManager;
            _repositoryParent = repositoryParent;
            _repositoryStudentParent = repositoryStudentParent;
            _systemSettingManager = systemSettingManager;
            _hashids = hashids;
            _repositoryStudentBillReportHistory = repositoryStudentBillReportHistory;
            _academicPeriodMetaDataManager = academicPeriodMetaDataManager;
            _studentBillActorProvider = studentBillActorProvider();
        }

        public async Task Delete(Guid id)
        {
            var bill = await _repositoryStudentBill.FirstOrDefaultAsync(x => x.Id == id && (x.BillStatus == (int)RhyoliteOrbitConsts.BillStatus.Draft || x.BillStatus == (int)RhyoliteOrbitConsts.BillStatus.Pending));
            if (bill != null)
            {
                var statementRecord = await _repositoryStudentStatement.FirstOrDefaultAsync(x => x.StudentId == bill.StudentId);
                var statementList = statementRecord.Statement;
                var recordToDelete = statementList.FirstOrDefault(a => a.ReferenceNo == bill.BillNo);
                statementList.Remove(recordToDelete);
                statementRecord.Statement = statementList;
                await _repositoryStudentStatement.UpdateAsync(statementRecord);
                await _repositoryStudentBill.DeleteAsync(id);
            }

        }
        

        public async Task<object> ListAll(Guid academicYearId, Guid termId, Guid classId, Guid billTypeId)
        {
            return await _repositoryStudentBill.GetAllListAsync(x=>x.AcademicYearId == academicYearId && x.TermId == termId && x.ClassId == classId && x.BillTypeId == billTypeId);
        }
        
        public async Task<object> ListBillHistory(int pageNo, int pageSize, string query, Guid classId)
        {
            var predicate = PredicateBuilder.New<StudentBillReportHistory>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.BillNo.ToLower().Contains(query.ToLower()));
            }
            
            if (classId != Guid.Empty)
            {
                predicate.And(x => x.ClassId  == classId);
            }
            
            var totalCount =
                
                await _repositoryStudentBillReportHistory.CountAsync(predicate);
            
            var data = await (await _repositoryStudentBillReportHistory.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.AcademicYearId, a.BillDate, a.BillNo, a.BillSetupId, a.BillTypeId, a.AcademicYearName, a.TermId, a.ClassId, a.TermName, a.ClassName, a.DocumentUrl, a.CreationTime, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
 
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

        public async Task<object> ListBillsToCancel(Guid academicYearId, Guid termId, Guid classId, Guid billTypeId)
        {
            return await _repositoryStudentBill.GetAllListAsync(x => x.AcademicYearId == academicYearId && x.TermId == termId && x.ClassId == classId && x.BillTypeId == billTypeId && x.BillStatus == 301);
        }

        public async Task<object> ListOpeningBalancesToCancel(Guid academicYearId, Guid termId, Guid classId)
        {
            return await _repositoryStudentBill.GetAllListAsync(x => x.AcademicYearId == academicYearId && x.TermId == termId && x.ClassId == classId && x.BillTypeId == Guid.Empty && x.BillSetupId == Guid.Empty);
        }

        public async Task<object> GetCurrentBillDetails(Guid acaYrId, Guid termId, Guid classId, Guid billTypeId)
        {
            return await _repositoryStudentBill.FirstOrDefaultAsync(a => a.AcademicYearId == acaYrId && a.TermId == termId && a.ClassId == classId && a.BillTypeId == billTypeId);
        }
        

        public async Task CreateBatchBills(StudentBillBatchDto dto)
        {
            List<StudentBillDto> billReports = new List<StudentBillDto>();
            
            Currency currency = null;
            
            var systemSetting = await (await _repositorySystemSetting.GetAllAsync()).FirstOrDefaultAsync();
            if (systemSetting != null)
            {
                currency = await _repositoryCurrency.FirstOrDefaultAsync(a => a.Id == systemSetting.DefaultCurrencyId);
            }

            var billSetUp = await _repositoryBillSetup.FirstOrDefaultAsync(x => x.Id == dto.BillSetupId);
            
            // transform billsetup details to bill details
             
            
            var billType = await _repositoryBillType.FirstOrDefaultAsync(a => a.Id == dto.BillTypeId);

            // get all students in the class
            var students = await _repositoryStudent.GetAllListAsync(x => x.ClassId == dto.ClassId);

            foreach (var student in students)
            {
                var metaData = await _academicPeriodMetaDataManager.GetAcademicPeriodMeta(dto.AcademicYearId, dto.TermId, dto.ClassId, student.Id);

                if (metaData == null)
                    continue;

                var description = $"Bill for {metaData.ClassName} - {metaData.AcademicYearName} {metaData.TermName}";
                
                // transform bill setup details to bill details
                var billDetails = TransformBillSetupDetails(
                    billSetUp.Details, 
                    student.Gender, 
                    billType.Id, 
                    billType.Name
                );

                var studentBillEntity = new StudentBill
                {
                    AcademicYearId = dto.AcademicYearId,
                    AcademicYearName = metaData.AcademicYearName,
                    TermId = dto.TermId,
                    TermName = metaData.TermName,
                    ClassId = dto.ClassId,
                    ClassName = metaData.ClassName,
                    StudentId = student.Id,
                    StudentIdentifier = metaData.StudentIdentifier,
                    StudentName = metaData.StudentName,
                    BillTypeId = dto.BillTypeId,
                    BillTypeName = billType.Name,
                    BillSetupId = dto.BillSetupId,
                    BillNo = dto.BillNo,
                    BillStatus = (int)RhyoliteOrbitConsts.BillStatus.Pending,
                    Description = description,
                    BillDate = dto.BillDate,
                    BillAmount = billDetails.Sum(x => x.FeeAmount),  // sum fees
                    BillBalance = billDetails.Sum(x => x.FeeAmount),
                    Details = billDetails,
                    CurrencyName = currency?.Name ?? "Ghana Cedi",
                    CurrencyCode = currency?.Code ?? "GHS",
                    MinorName = currency?.MinorName,
                    BuyRate = currency?.BuyRate ?? 1,
                    SellRate = currency?.SellRate ?? 1
                };

                // map setup info
                if (billSetUp != null)
                {
                    var billSetupInfo = _mapper.Map<BillSetupInfo>(billSetUp);
                    studentBillEntity.BillSetupInfo = billSetupInfo;
                }

                await _repositoryStudentBill.InsertAsync(studentBillEntity);

                // update student statement
                var existingBills = await _repositoryStudentBill.GetAllListAsync(x => x.StudentId == student.Id);

                var currentStatement = await _repositoryStudentStatement.FirstOrDefaultAsync(a => a.StudentId == student.Id);

                var statementRecord = new List<Statement>
                {
                    new Statement
                    {
                        Id = Guid.NewGuid(),
                        ActivityDate = studentBillEntity.BillDate.ToString("dd-MMM-yyyy"),
                        ReferenceNo = studentBillEntity.BillNo,
                        Description = studentBillEntity.Description,
                        Invoice = studentBillEntity.BillAmount,
                        Payment = 0,
                        Balance = existingBills.Sum(x => x.BillBalance),
                        CurrencyName = currency?.Name ?? "GHS",
                        TransactionType = 1,
                        StudentIdentifier = metaData.StudentIdentifier,
                        StudentName = metaData.StudentName
                    }
                };

                if (currentStatement != null)
                {
                    currentStatement.Statement.AddRange(statementRecord);
                    await _repositoryStudentStatement.UpdateAsync(currentStatement);
                }
                else
                {
                    await _repositoryStudentStatement.InsertAsync(new StudentStatement
                    {
                        StudentId = student.Id,
                        Statement = statementRecord,
                        TenantId = student.TenantId
                    });
                }
                
                // compute balance brought forward
                var pastBills = await _repositoryStudentBill.GetAllListAsync(x=> x.StudentId == student.Id && x.BillDate < DateTime.UtcNow && x.BillBalance > 0);

                //prepare actor messages.
                
                var studentBill = new StudentBillDto
                {
                    ReportHeader = new BillReportHeader
                    {
                        SchoolName = "Christ Royal International School",
                        PrimaryPhoneNo = "0548043779",
                        SchAddress = "",
                        LogoUrl = "https://res.cloudinary.com/rhyoliteprime/image/upload/v1673255393/school_manager_repository/cris-logo.jpg",
                        Email = "Christ.royal.sch@gmail.com",
                        IsBalanceBfHandledManually = dto.IsBalanceBfHandledManually,
                    },
                    BillMetaData = new BillMetaData
                    {
                        AcademicYearId = dto.AcademicYearId,
                        TermId = dto.TermId,
                        ClassId = dto.ClassId,
                        BillTypeId = dto.BillTypeId, 
                        BillDate = dto.BillDate, 
                        BillNo = dto.BillNo, 
                        BillSetupId = dto.BillSetupId
                    },
                    StudentInfo = new StudentInfo
                    {
                        FullName = $"{student.FirstName} {student.LastName} {student.MiddleName}",
                        EnrollmentDate = student.EnrollmentDate.ToString(CultureInfo.InvariantCulture),
                        StudentIdentifier = student.StudentIdentifier,
                        Gender = student.Gender,
                        ClassName = student.ClassName,
                        DateOfBirth = student.DateOfBirth.ToString(CultureInfo.InvariantCulture),
                        Id = student.Id
                    },
                    StudentBill = new StudentBillDetail
                    {
                        BillNo = dto.BillNo,
                        BillDate = dto.BillDate,
                        Period = "2nd Term, 2025/2026",
                        CurrencyName = "Ghana Cedi",
                        CurrencyCode = "GHS",
                        BillAmount = billDetails.Sum(x => x.FeeAmount),
                        BillBalance = billDetails.Sum(x => x.FeeAmount),
                        BalanceBF = pastBills.Sum(a=> a.BillBalance),
                        PaymentChannels = ["Bank", "USSD"],
                        Details = billDetails.Select(x => new BillItem
                        {
                            Id = x.Id,
                            FeeId = x.FeeId,
                            FeeName = x.FeeName,
                            BillTypeId = x.BillTypeId,
                            BillTypeName = x.BillTypeName,
                            FeeAmount = x.FeeAmount
                        }).ToList()

                    },
                    TenantId = billSetUp.TenantId
                };
            
                billReports.Add(studentBill);
                
            }

            if (billReports.Any())
            {
                _studentBillActorProvider.Tell(billReports);
            }
            
            
            
        }
        
        
        private List<BillDetail> TransformBillSetupDetails(
            List<BillSetupDetail> setupDetails, 
            string studentGender, 
            Guid billTypeId, 
            string billTypeName)
        {
            if (string.IsNullOrWhiteSpace(studentGender))
                studentGender = "Unknown";

            return setupDetails
                .Where(d => string.IsNullOrEmpty(d.Gender) || d.Gender.Equals(studentGender, StringComparison.OrdinalIgnoreCase))
                .Select(d => new BillDetail
                {
                    Id = Guid.NewGuid(),
                    FeeId = d.FeeId,
                    FeeName = d.FeeName,
                    BillTypeId = billTypeId,
                    BillTypeName = billTypeName,
                    FeeAmount = d.FeeAmount
                })
                .ToList();
        }
        
        public async Task CreateOpeningBalance(StudentBill studentBill)
        {
            Currency currency = null;
            var systemSetting = await (await _repositorySystemSetting.GetAllAsync()).FirstOrDefaultAsync();
            if (systemSetting != null)
            {
                currency = await _repositoryCurrency.FirstOrDefaultAsync(a => a.Id == systemSetting.DefaultCurrencyId);
            }

            var metaData = await _academicPeriodMetaDataManager.GetAcademicPeriodMeta(studentBill.AcademicYearId, studentBill.TermId, studentBill.ClassId, studentBill.StudentId);

            if (metaData != null)
            {
                studentBill.ClassName = metaData.ClassName;
                studentBill.StudentIdentifier = metaData.StudentIdentifier;
                studentBill.StudentName = metaData.StudentName;
                studentBill.AcademicYearName = metaData.AcademicYearName;
                studentBill.TermName = metaData.TermName;
                studentBill.Description = $"Opening Balance For {metaData.AcademicYearName}-{metaData.TermName}";
            }


            if (currency != null)
            {
                studentBill.CurrencyName = currency.Name;
                studentBill.CurrencyCode = currency.Code;
                studentBill.MinorName = currency.MinorName;
                studentBill.BuyRate = currency.BuyRate;
                studentBill.SellRate = currency.SellRate;
            }

            var billId = Guid.NewGuid();
            studentBill.Id = billId;

            await _repositoryStudentBill.InsertAsync(studentBill);

            //create statement record

            var bill =
                await _repositoryStudentBill.GetAllListAsync(x => x.StudentId == studentBill.StudentId);

            var currentStatement =
                await _repositoryStudentStatement.FirstOrDefaultAsync(a => a.StudentId == studentBill.StudentId);

            var statementRecord = new List<Statement>();
            statementRecord.Add(new Statement
            {
                Id = billId,
                ActivityDate = studentBill.BillDate.ToString("dd-MMM-yyyy"),
                ReferenceNo = studentBill.BillNo,
                Description = studentBill.Description,
                Invoice = studentBill.BillAmount,
                Payment = 0,
                Balance = bill.Sum(x => x.BillBalance) + studentBill.BillBalance,
                CurrencyName = currency == null ? "GHS" : currency.Name,
                StudentIdentifier = metaData.StudentIdentifier,
                StudentName = metaData.StudentName,
                TransactionType = 1
            });

            if (currentStatement != null)
            {

                currentStatement.Statement.AddRange(statementRecord);

                await _repositoryStudentStatement.UpdateAsync(currentStatement);
            }
            else
            {
                await _repositoryStudentStatement.InsertAsync(new StudentStatement
                {
                    StudentId = studentBill.StudentId,
                    Statement = statementRecord,
                    TenantId = studentBill.TenantId,
                });
            }


        }

        public async Task CancelBill(StudentBill bill)
        {

            await _repositoryCancelledBill.InsertAsync(new CancelledBill
            {

                ClassId = bill.ClassId,
                ClassName = bill.ClassName,
                AcademicYearId = bill.AcademicYearId,
                AcademicYearName = bill.AcademicYearName,
                TermId = bill.TermId,
                TermName = bill.TermName,
                StudentId = bill.StudentId,
                StudentIdentifier = bill.StudentIdentifier,
                StudentName = bill.StudentName,
                BillAmount = bill.BillAmount,
                BillBalance = bill.BillBalance,
                BillDate = bill.BillDate,
                BillNo = bill.BillNo,
                BillSetupId = bill.BillSetupId,
                BillSetupInfo = bill.BillSetupInfo,
                BillStatus = bill.BillStatus,
                BillTypeId = bill.BillTypeId,
                BillTypeName = bill.BillTypeName,
                Details = bill.Details,
                BuyRate = bill.BuyRate,
                SellRate = bill.SellRate,
                CurrencyCode = bill.CurrencyCode,
                CurrencyName = bill.CurrencyName,
                MinorName = bill.MinorName,
                Description = bill.Description,
            });

            await _repositoryStudentBill.DeleteAsync(bill.Id);

            //update statement
            var currentStatement =
                   await _repositoryStudentStatement.FirstOrDefaultAsync(a => a.StudentId == bill.StudentId);


            if (currentStatement != null)
            {
                //pull bill No record from statement

                var statementList = currentStatement.Statement;

                var invoiceOnStatement = currentStatement.Statement.FirstOrDefault(x => x.Id == bill.Id);

                currentStatement.Statement.Remove(invoiceOnStatement);

                await _repositoryStudentStatement.UpdateAsync(currentStatement);

            }

        }
        
        public async Task<object> ListAllBalances(Guid academicYearId, Guid termId, Guid classId)
        {
            return await _repositoryStudentBill.GetAllListAsync(x => x.AcademicYearId == academicYearId && x.TermId == termId && x.ClassId == classId);
        }

        public async Task<object> ListStudentBills(Guid studentId)
        {
            return await _repositoryStudentBill.GetAllListAsync(x => x.StudentId == studentId && Math.Round(x.BillBalance,2) > 0);
        }

        public async Task<object> GetBill(Guid id)
        {
            return await _repositoryStudentBill.GetAsync(id);
        }


        public async Task<object> GetPaidUpStudents(Guid academicYearId, Guid termId, Guid classId)
        {
            return await _repositoryStudentBill.GetAllListAsync(x => x.AcademicYearId == academicYearId && x.TermId == termId && x.ClassId == classId && x.BillBalance <= 0);
        }

        public async Task<object> GetStudentDebtors(Guid academicYearId, Guid termId, Guid classId)
        {
            return await _repositoryStudentBill.GetAllListAsync(x => x.AcademicYearId == academicYearId && x.TermId == termId && x.ClassId == classId && x.BillBalance > 0);

        }
        
        // bill report updates

        
        public async Task UpdateBillWithDocumentUrl(StudentBillReportUpdateDto dto)
        {
            using (_unitOfWorkManager.Current.SetTenantId(dto.TenantId))
            {
                foreach (var record in dto.StudentBillReportList)
                {
                    var studentBillRecord = await _repositoryStudentBill.FirstOrDefaultAsync(a =>
                        a.StudentId == record.StudentId &&
                        a.AcademicYearId == dto.BillMetaData.AcademicYearId &&
                        a.TermId == dto.BillMetaData.TermId &&
                        a.ClassId == dto.BillMetaData.ClassId &&
                        a.BillNo == dto.BillMetaData.BillNo);

                    if (studentBillRecord != null)
                    {
                        studentBillRecord.DocumentUrl = record.BillReportUrl;
                        await _repositoryStudentBill.UpdateAsync(studentBillRecord);
                    }
                    
                }
                
                var metaData = await _academicPeriodMetaDataManager.GetAcademicPeriodMeta(dto.BillMetaData.AcademicYearId, dto.BillMetaData.TermId, dto.BillMetaData.ClassId);

                
                // Check if the StudentBillReportHistory already exists
                var existingReportHistory = await _repositoryStudentBillReportHistory.FirstOrDefaultAsync(x =>
                    x.AcademicYearId == dto.BillMetaData.AcademicYearId &&
                    x.TermId == dto.BillMetaData.TermId &&
                    x.ClassId == dto.BillMetaData.ClassId &&
                    x.BillTypeId == dto.BillMetaData.BillTypeId &&
                    x.BillNo == dto.BillMetaData.BillNo &&
                    x.TenantId == dto.TenantId);

                if (existingReportHistory != null)
                {
                    // Only update the DocumentUrl
                    existingReportHistory.DocumentUrl = dto.BatchDocumentUrl;
                    
                    await _repositoryStudentBillReportHistory.UpdateAsync(existingReportHistory);
                }
                else
                {
                    // Create new record if it doesn't exist
                    var studentBillReportHistory = new StudentBillReportHistory
                    {
                        AcademicYearId = dto.BillMetaData.AcademicYearId,
                        AcademicYearName = metaData.AcademicYearName,
                        TermId = dto.BillMetaData.TermId,
                        TermName = metaData.TermName,
                        ClassId = dto.BillMetaData.ClassId,
                        ClassName = metaData.ClassName,
                        BillTypeId = dto.BillMetaData.BillTypeId,
                        BillSetupId = dto.BillMetaData.BillSetupId,
                        BillDate = dto.BillMetaData.BillDate,
                        BillNo = dto.BillMetaData.BillNo,
                        DocumentUrl = dto.BatchDocumentUrl,
                        TenantId = dto.TenantId
                    };
                    await _repositoryStudentBillReportHistory.InsertAsync(studentBillReportHistory);
                }
            }
        }
        
        
        //for student self service
        public async Task<object> GetStudentBillSummary(string studentId, string pin, int tenantId)
        {

            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                string hashedPin = _hashids.Encode(int.Parse(pin));
                
                var studentInfo = await _repositoryStudent.FirstOrDefaultAsync(a =>
                    a.StudentIdentifier.Trim() == studentId.Trim() && a.Pin == hashedPin);

                if (studentInfo == null)
                {
                    return new
                    {
                        Summary = "The Student ID or PIN you entered is incorrect.\nPlease double-check your information and try again.",
                    };
                }
            
                var bills = await _repositoryStudentBill.GetAllListAsync(x => x.StudentId == studentInfo.Id && x.BillBalance > 0);
                
                var defaultCurrencyCode = await _systemSettingManager.GetDefaultCurrencyCode();
                //use string builder to build the summary based on the format
                //add last payment date.
                var studentName = bills.FirstOrDefault()?.StudentName;
                var totalBalance = bills.Sum(bill => bill.BillBalance);

                StringBuilder summaryBuilder = new StringBuilder();
                summaryBuilder.AppendLine($"*** Student Bill Summary ***\n\nStudent Name: {studentName}\nStudent ID: {studentId}\nOutstanding Balance: {defaultCurrencyCode} {totalBalance:N2}");

                string summary = summaryBuilder.ToString();

                return new
                {
                    StudentName = studentName,
                    StudentId = studentId,
                    TotalBillBalance = totalBalance,
                    Summary = totalBalance > 0 ? summary : "You currently have no outstanding tuition fees\nCome back later to check your bill summary.",
                    studentInfo.Id
                };
                
            }


        }
        
         
        
       public async Task<object> GetParentWardsBillSummary(string phoneNo, int tenantId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var parentInfo = await _repositoryParent.FirstOrDefaultAsync(a =>
                    a.FirstGuardianPhoneNo.Trim() == phoneNo.Trim() ||
                    a.SecondGuardianPhoneNo.Trim() == phoneNo.Trim());
        
                if (parentInfo == null)
                    return null;
        
                var studentsParents = await _repositoryStudentParent.GetAllListAsync(x => x.ParentId == parentInfo.Id);
                var studentIds = studentsParents.Select(x => x.StudentId).ToList();
        
                var totalBillBalance = 0m;
                var summaryBuilder = new StringBuilder();
        
                foreach (var studentId in studentIds)
                {
                    var bills = await _repositoryStudentBill.GetAllListAsync(x => x.StudentId == studentId && x.BillBalance > 0);
                    var studentName = bills.FirstOrDefault()?.StudentName ?? "Unknown";
                    var billBalance = bills.Sum(bill => bill.BillBalance);
                    totalBillBalance += billBalance;
        
                    summaryBuilder.AppendLine(
                        $"Student Name: {studentName}\nStudent ID: {studentId}\nBill Amount: {billBalance:C}\n"
                    );
                }
        
                var billSummary = summaryBuilder.Length > 0
                    ? summaryBuilder.ToString()
                    : "No outstanding bills for any wards.";
        
                return new
                {
                    BillSummary = billSummary,
                    TotalBillBalance = totalBillBalance
                };
            }
        }
       
        // bill summary by bill type
        
        public async Task<object> GetBillSummaryByBillType(string studentId, string pin, string billType, int tenantId)
        {

            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                
                var studentInfo = await _repositoryStudent.FirstOrDefaultAsync(a =>
                    a.StudentIdentifier.Trim() == studentId.Trim());
                
                if (studentInfo == null)
                {
                    return null;
                }
                
                string normalizedBillType = billType.ToLower().Replace(" ", "");
                
                var defaultCurrencyCode = await _systemSettingManager.GetDefaultCurrencyCode();
            
                var bills = await _repositoryStudentBill.GetAllListAsync(x => x.StudentId == studentInfo.Id  && x.BillTypeName.ToLower().Replace(" ", "") == normalizedBillType && x.BillBalance > 0);
            
                var studentName = string.IsNullOrEmpty(studentInfo.MiddleName) ? $"{studentInfo.FirstName} {studentInfo.LastName}" : $"{studentInfo.FirstName} {studentInfo.MiddleName} {studentInfo.LastName}";
            
                var totalBillBalance = bills.Sum(bill => bill.BillBalance);
                var totalBillAmount = bills.Sum(bill => bill.BillAmount);
            
                return new
                {
                    studentInfo.Id,
                    studentName,
                    studentId,
                    totalBillBalance,
                    totalBillAmount,
                    Summary = totalBillBalance > 0 ? $"Student Name: {studentName}\nStudent ID: {studentId}\nBill Amount: {defaultCurrencyCode} {totalBillAmount:N2}\nBill Balance: {defaultCurrencyCode} {totalBillBalance:N2}" : $"You currently have no outstanding tuition fees\nCome back later to check your bill summary."
                };
                
            }

            
        }
}