using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using HashidsNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RhyoliteOrbit.DomainServices.Shared.MetaData;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Models.Shared;
using RhyoliteOrbit.Redis;
using RhyoliteOrbit.Utils;

namespace RhyoliteOrbit.DomainServices.SchoolManager.BillPayments;

public class BillPaymentManager: Abp.Domain.Services.DomainService, IBillPaymentManager
{
        private readonly IRepository<BillPayment,Guid> _repositoryBillPayment;
        private readonly IRepository<Student, Guid> _repositoryStudent;
        private readonly IRepository<SchoolProfile, Guid> _repositorySchoolProfile;
        private readonly IRepository<StudentBill, Guid> _repositoryStudentBill;
        private readonly IRepository<CancelledPayment, Guid> _repositoryCancelledPayment;
        private readonly IRepository<Currency, Guid> _repositoryCurrency;
        private readonly IRepository<StudentStatement, Guid> _repositoryStudentStatement;
        private readonly IRepository<StudentBillReceipt, Guid> _repositoryStudentBillReceipt;
        private readonly IRepository<StudentParent, Guid> _repositoryStudentParent;
        private readonly IRepository<SystemSetting, Guid> _repositorySystemSetting;
        private readonly IAcademicPeriodMetaDataManager _academicPeriodMetaDataManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRedisCacheManager _redisCacheManager;
        private readonly IHashids _hashids;

        private readonly int _paymentReceiptDatabaseId;


        public BillPaymentManager(IRepository<BillPayment, Guid> repositoryBillPayment, IConfiguration configuration, IRepository<Student, Guid> repositoryStudent, IRepository<StudentBill, Guid> repositoryStudentBill, IRepository<Currency, Guid> repositoryCurrency, IRepository<StudentStatement, Guid> repositoryStudentStatement, IAcademicPeriodMetaDataManager academicPeriodMetaDataManager, IRepository<StudentBillReceipt, Guid> repositoryStudentBillReceipt, IRepository<StudentParent, Guid> repositoryStudentParent, IRepository<SystemSetting, Guid> repositorySystemSetting, IRepository<CancelledPayment, Guid> repositoryCancelledPayment, IRepository<SchoolProfile, Guid> repositorySchoolProfile, IRedisCacheManager redisCacheManager, IHashids hashids, IUnitOfWorkManager unitOfWorkManager)
        {
            _repositoryBillPayment = repositoryBillPayment;
            _repositoryStudent = repositoryStudent;
            _repositoryStudentBill = repositoryStudentBill;
            _repositoryCurrency = repositoryCurrency;
            _repositoryStudentStatement = repositoryStudentStatement;
            _academicPeriodMetaDataManager = academicPeriodMetaDataManager;
            _repositoryStudentBillReceipt = repositoryStudentBillReceipt;
            _repositoryStudentParent = repositoryStudentParent;
            _repositorySystemSetting = repositorySystemSetting;
            _repositoryCancelledPayment = repositoryCancelledPayment;
            _repositorySchoolProfile = repositorySchoolProfile;
            _redisCacheManager = redisCacheManager;
            _paymentReceiptDatabaseId = Convert.ToInt32(configuration["RedisCache:PaymentReceiptDatabaseId"]);
            _hashids = hashids;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<object> Create(BillPayment entity)
        {

            Dictionary<string, object> kv = new Dictionary<string, object>();
            
            if (entity.AmountPaid > 0)
            {
                var systemSetting = await (await _repositorySystemSetting.GetAllAsync()).FirstOrDefaultAsync();

                var currency = await _repositoryCurrency.FirstOrDefaultAsync(entity.CurrencyId);
                
                var basicInfo = await _academicPeriodMetaDataManager.GetAcademicPeriodMeta(entity.AcademicYearId, entity.TermId, entity.ClassId, entity.StudentId);

                entity.CurrencyName = currency.Name;
                entity.CurrencyCode = currency.Code;
                entity.CurrencySellRate = currency.SellRate;
                entity.CurrencyBuyRate = currency.BuyRate;
                entity.StudentIdentifier = basicInfo.StudentIdentifier;
                entity.StudentName = basicInfo.StudentName;
                entity.TermName = basicInfo.TermName;
                entity.ClassName = basicInfo.ClassName;
                entity.AcademicYearName = basicInfo.AcademicYearName;

                var paymentId = await _repositoryBillPayment.InsertAndGetIdAsync(entity);
                
                //create statement record...
                
                var currentBill =
                    await _repositoryStudentBill.FirstOrDefaultAsync(x => x.StudentId == entity.StudentId && x.Id == entity.BillId);

                
                var currentStatement =
                    await _repositoryStudentStatement.FirstOrDefaultAsync(a => a.StudentId == entity.StudentId);

                var statementRecord = new List<Statement>
                {
                    new Statement
                    {
                        Id = paymentId,
                        ActivityDate = entity.PaymentDate.ToString("dd-MMM-yyyy"),
                        ReferenceNo = entity.ReceiptNo,
                        Description = entity.PaymentDescription,
                        Invoice = 0,
                        Payment = entity.AmountPaid,
                        Balance = currentBill.BillBalance - entity.AmountPaid,
                        CurrencyName = currency.Code,
                        TransactionType = 2,
                        StudentIdentifier = basicInfo.StudentIdentifier,
                        StudentName = basicInfo.StudentName,
                        StudentId = entity.StudentId,

                    }
                };


                if (currentStatement != null)
                {
                    var statementList = currentStatement.Statement;
                    statementList.AddRange(statementRecord);

                    await _repositoryStudentStatement.UpdateAsync(currentStatement);
                }
                else
                {
                    await _repositoryStudentStatement.InsertAsync(new StudentStatement
                    {
                        StudentId = entity.StudentId,
                        Statement = statementRecord,
                        StudentIdentifier= basicInfo.StudentIdentifier,
                        StudentName= basicInfo.StudentName,
                        TenantId = entity.TenantId,

                    });
                }

                //create receipt...

                var receiptRecord = await _repositoryStudentBillReceipt.FirstOrDefaultAsync(a => a.AcademicYearId == entity.AcademicYearId && a.TermId == entity.TermId && a.ClassId == entity.ClassId && a.ReceiptNo == entity.ReceiptNo);
                if (receiptRecord != null)
                {
                    receiptRecord.AmountPaid += entity.AmountPaid;
                    receiptRecord.BalanceDue += (currentBill.BillBalance - entity.AmountPaid);
                }
                else
                {
                    var receipt = new StudentBillReceipt
                    {
                        ReceivedFrom = basicInfo.StudentName,
                        AmountInWords = $"{new UtilService().ConvertToWords(entity.AmountPaid.ToString(CultureInfo.InvariantCulture))}Ghana Cedis Only",
                        AmountPaid = entity.AmountPaid,
                        Reason = entity.PaymentDescription,
                        BalanceDue = currentBill.BillBalance - entity.AmountPaid,
                        PaymentType = entity.ModeOfPayment,
                        StudentId = entity.StudentId,
                        StudentIdentifier = basicInfo.StudentIdentifier,
                        AcademicYearName = basicInfo.AcademicYearName,
                        TermName = basicInfo.TermName,
                        ReceiptNo = entity.ReceiptNo,
                        ClassName = basicInfo.ClassName,
                        PaymentId = paymentId,
                        AcademicYearId = entity.AcademicYearId,
                        TermId = entity.TermId,
                        ClassId = entity.ClassId,
                         
                    };

                    await _repositoryStudentBillReceipt.InsertAsync(receipt);
                    kv.Add("receipt", receipt);

                }

                var bill = await _repositoryStudentBill.FirstOrDefaultAsync(x => x.Id == entity.BillId);
                bill.BillBalance = currentBill.BillBalance - entity.AmountPaid;
                bill.BillStatus = 401;
                await _repositoryStudentBill.UpdateAsync(bill);

                var studParent =
                    await _repositoryStudentParent.FirstOrDefaultAsync(x => x.StudentId == entity.StudentId);
                if (studParent != null && systemSetting is { SendBillPaymentEmailNotification: true })
                {
                    var parent = await _repositoryStudent.FirstOrDefaultAsync(x => x.Id == studParent.ParentId);
                    kv.Add("recipient", parent);
                    
                    // produce to RabbitMQ consumer ... 
                    
                }

                var receiptKey = _hashids.Encode(new Random().Next());
                //set payment receipt keys to redis
                await _redisCacheManager.SetValueAsync( receiptKey, JsonConvert.SerializeObject(entity), _paymentReceiptDatabaseId);

                //set ttl on key
                var ttl = DateTime.UtcNow.AddDays(1) - DateTime.UtcNow;
                await _redisCacheManager.SetExpireTimeAsync(receiptKey, ttl, _paymentReceiptDatabaseId);

                return receiptKey;
            }

            return null;
        }

        public async Task<object> CreateCreditMemo(BillPayment entity)
        {

            Dictionary<string, object> kv = new Dictionary<string, object>();

            var systemSetting = await (await _repositorySystemSetting.GetAllAsync()).FirstOrDefaultAsync();
            
            var currency = await _repositoryCurrency.FirstOrDefaultAsync(entity.CurrencyId);

            var basicInfo = await _academicPeriodMetaDataManager.GetAcademicPeriodMeta(entity.AcademicYearId, entity.TermId, entity.ClassId, entity.StudentId);

            var billHeaderId = await _repositoryStudentBill.InsertAndGetIdAsync(new StudentBill
            {
                AcademicYearId = entity.AcademicYearId,
                AcademicYearName = basicInfo.AcademicYearName,
                TermId = entity.TermId,
                TermName = basicInfo.TermName,
                ClassId = entity.ClassId,
                ClassName = basicInfo.ClassName,
                BillNo = entity.BillNo.Trim(),
                BillDate = DateTime.UtcNow,
                CurrencyName = currency == null ? "Ghana Cedi" : currency.Name,
                CurrencyCode = currency == null ? "GHS" : currency.Code,
                MinorName = currency == null ? "Pesewas" : currency.MinorName,
                BuyRate = currency?.BuyRate ?? 1,
                SellRate = currency?.SellRate ?? 1,
                BillAmount = 0,
                BillBalance = -1 * entity.AmountPaid,
                StudentId = entity.StudentId,
                StudentName = basicInfo.StudentName,
                StudentIdentifier = basicInfo.StudentIdentifier,
                BillStatus = 301,
                Description = entity.PaymentDescription,
                BillTypeId = Guid.Empty,
                BillSetupId = Guid.Empty,
                TenantId = entity.TenantId,
                BillSetupInfo = null,
                Details = null

            });

            var paymentId = await _repositoryBillPayment.InsertAndGetIdAsync(new BillPayment
            {
                AcademicYearId = entity.AcademicYearId,
                AcademicYearName = basicInfo.AcademicYearName,
                TermId = entity.TermId,
                TermName = basicInfo.TermName,
                ClassId = entity.ClassId,
                ClassName = basicInfo.ClassName,
                StudentId = entity.StudentId,
                StudentName = basicInfo.StudentName,
                StudentIdentifier = basicInfo.StudentIdentifier,
                AmountPaid = entity.AmountPaid,
                ModeOfPayment = entity.ModeOfPayment,
                PaymentDate = entity.PaymentDate,
                CurrencyId = entity.CurrencyId,
                CurrencyBuyRate = currency?.BuyRate ?? 1,
                CurrencySellRate = currency?.SellRate ?? 1,
                ChequeNo = entity.ChequeNo,
                BillId = billHeaderId,
                BillNo = entity.BillNo,
                ReceiptNo = entity.ReceiptNo,
                PaymentDescription = entity.PaymentDescription,
                IsCreditMemo = entity.IsCreditMemo,
                IsPosted = false,
                CurrencyName = currency == null ? "Ghana Cedi" : currency.Name,
                CurrencyCode = currency == null ? "GHS" : currency.Code,
                TenantId = entity.TenantId,
            });

            await CurrentUnitOfWork.SaveChangesAsync();


            //create statement record ...
            var bills =
                await _repositoryBillPayment.GetAllListAsync(x => x.StudentId == entity.StudentId);

            var currentStatement =
                await _repositoryStudentStatement.FirstOrDefaultAsync(a => a.StudentId == entity.StudentId);

            var statementRecord = new List<Statement>();
            statementRecord.Add(new Statement
            {
                Id = paymentId,
                ActivityDate = entity.PaymentDate.ToString("dd-MMM-yyyy"),
                ReferenceNo = entity.ReceiptNo,
                Description = entity.PaymentDescription,
                Invoice = 0,
                Payment = entity.AmountPaid,
                Balance = -1 * entity.AmountPaid,
                CurrencyName = currency == null ? "GHS" : currency.Code,
                TransactionType = 2,
                StudentIdentifier = basicInfo.StudentIdentifier,
                StudentName = basicInfo.StudentName,
                StudentId = entity.StudentId,

            });

            if (currentStatement != null)
            {
                var statementList = currentStatement.Statement;
                statementList.AddRange(statementRecord);

                await _repositoryStudentStatement.UpdateAsync(currentStatement);
            }
            else
            {
                await _repositoryStudentStatement.InsertAsync(new StudentStatement
                {
                    StudentId = entity.StudentId,
                    Statement = statementRecord,
                });
            }

            var receiptRecord = await _repositoryStudentBillReceipt.FirstOrDefaultAsync(a => a.AcademicYearId == entity.AcademicYearId && a.TermId == entity.TermId && a.ClassId == entity.ClassId && a.ReceiptNo == entity.ReceiptNo);
            
            if (receiptRecord != null)
            {
                receiptRecord.AmountPaid += entity.AmountPaid;
                receiptRecord.BalanceDue += (-1 * entity.AmountPaid);

                await _repositoryStudentBillReceipt.UpdateAsync(receiptRecord);
            }
            else
            {
                var receipt = new StudentBillReceipt
                {
                    ReceivedFrom = basicInfo.StudentName,
                    AmountInWords = $"{new UtilService().ConvertToWords(entity.AmountPaid.ToString(CultureInfo.InvariantCulture))}Ghana Cedis Only",
                    AmountPaid = entity.AmountPaid,
                    Reason = entity.PaymentDescription,
                    BalanceDue = -1 * entity.AmountPaid,
                    PaymentType = entity.ModeOfPayment,
                    StudentId = entity.StudentId,
                    StudentIdentifier = basicInfo.StudentIdentifier,
                    AcademicYearName = basicInfo.AcademicYearName,
                    TermName = entity.TermName,
                    ReceiptNo = entity.ReceiptNo,
                    ClassName = basicInfo.ClassName,
                    PaymentId = paymentId,
                    AcademicYearId = entity.AcademicYearId,
                    TermId = entity.TermId,
                    ClassId = entity.ClassId,

                };

                await _repositoryStudentBillReceipt.InsertAsync(receipt);

                kv.Add("receipt", receipt);

            }

            var studParent =
                await _repositoryStudentParent.FirstOrDefaultAsync(x => x.StudentId == entity.StudentId);

            if (studParent != null && systemSetting.SendBillPaymentEmailNotification)
            {
                var parent = await _repositoryStudent.FirstOrDefaultAsync(x => x.Id == studParent.ParentId);
                kv.Add("recipient", parent);

                // produce to RabbitMQ consumer via an actor
            }


            var receiptKey = _hashids.Encode(new Random().Next());
            //set payment receipt keys to redis
            await _redisCacheManager.SetValueAsync( receiptKey, JsonConvert.SerializeObject(entity),_paymentReceiptDatabaseId);

            //set ttl on key
            var ttl = DateTime.UtcNow.AddDays(1) - DateTime.UtcNow;
            await _redisCacheManager.SetExpireTimeAsync(receiptKey, ttl, _paymentReceiptDatabaseId);

            return receiptKey;

        }
        
        
        public async Task<IEnumerable<object>> ListAll(Guid academicYearId, Guid termId, Guid classId)
        {
            return await _repositoryBillPayment.GetAllListAsync(a => a.AcademicYearId == academicYearId && a.TermId == termId && a.ClassId == classId);

        }

        public async Task<IEnumerable<object>> ListAllCreditMemos(Guid academicYearId, Guid termId, Guid classId)
        {
            return await _repositoryBillPayment.GetAllListAsync(a => a.AcademicYearId == academicYearId && a.TermId == termId && a.ClassId == classId && a.IsCreditMemo);

        }

        public async Task<IEnumerable<object>> ListDailyPayments(DateTime paymentDate)
        {
            return await _repositoryBillPayment.GetAllListAsync(a => a.PaymentDate.Date == paymentDate.Date);

        }

        public async Task CancelPayment(BillPayment payment)
        {
            await _repositoryCancelledPayment.InsertAsync(new CancelledPayment { 
            
                AcademicYearId = payment.AcademicYearId,
                AcademicYearName = payment.AcademicYearName,
                TermId = payment.TermId,
                TermName = payment.TermName,
                ClassId = payment.ClassId,
                ClassName = payment.ClassName,
                StudentId = payment.StudentId,
                StudentIdentifier = payment.StudentIdentifier,
                StudentName = payment.StudentName,
                AmountPaid = payment.AmountPaid,
                ModeOfPayment = payment.ModeOfPayment,
                PaymentDate = payment.PaymentDate,
                CurrencyId = payment.CurrencyId,
                CurrencyName = payment.CurrencyName,
                CurrencyCode = payment.CurrencyCode,
                CurrencyBuyRate = payment.CurrencyBuyRate,
                CurrencySellRate = payment.CurrencySellRate,
                ChequeNo = payment.ChequeNo,
                BillId = payment.BillId,
                BillNo = payment.BillNo,
                ReceiptNo = payment.ReceiptNo,
                PaymentDescription = payment.PaymentDescription,
                IsCreditMemo = payment.IsCreditMemo,
                IsPosted = payment.IsPosted,

            });

            await _repositoryBillPayment.DeleteAsync(payment.Id);

            //update bill balance
            var studentBill = await _repositoryStudentBill.FirstOrDefaultAsync(payment.BillId);
            studentBill.BillBalance += payment.AmountPaid;
            studentBill.BillStatus = 301;
            await _repositoryStudentBill.UpdateAsync(studentBill);

           

            //update statement
            var currentStatement =
                   await _repositoryStudentStatement.FirstOrDefaultAsync(a => a.StudentId == payment.StudentId);
            

            if (currentStatement != null)
            {
                //pull bill No record from statement
                var bills = await _repositoryStudentBill.GetAllListAsync(x => x.StudentId == studentBill.StudentId);

                var statementList = currentStatement.Statement;

                var invoiceOnStatement = statementList.FirstOrDefault(x => x.Id == payment.BillId);

                var index = statementList.FindIndex(x => x.Id == payment.BillId);

                statementList.Remove(invoiceOnStatement);

                invoiceOnStatement.Payment = 0;
                invoiceOnStatement.Balance = bills.Sum(x => x.BillBalance);

                var paymentOnStatement = statementList.FirstOrDefault(x => x.Id == payment.Id);

                currentStatement.Statement.Remove(paymentOnStatement);

                if (statementList.Any())
                {
                    currentStatement.Statement[index] = invoiceOnStatement;
                }
                else
                {
                    currentStatement.Statement.Add(invoiceOnStatement);
                }

                await _repositoryStudentStatement.UpdateAsync(currentStatement);

            }

        }

        public async Task<IEnumerable<object>> ListAll(Guid academicYearId, Guid termId, Guid classId, Guid studentId)
        {
            return await _repositoryBillPayment.GetAllListAsync(a => a.AcademicYearId == academicYearId && a.TermId == termId && a.ClassId == classId && a.StudentId == studentId);
        }


        public async Task<IEnumerable<object>> ListAllUnPosted(Guid academicYearId, Guid termId, Guid classId)
        {
            return await _repositoryBillPayment.GetAllListAsync(a => a.AcademicYearId == academicYearId && a.TermId == termId && a.ClassId == classId && !a.IsPosted);
        }


        public async Task Delete(Guid id)
        {
            //delete receipts
            var receipt = await _repositoryStudentBillReceipt.FirstOrDefaultAsync(x => x.PaymentId == id);
            await _repositoryStudentBillReceipt.DeleteAsync(receipt);
            await _repositoryBillPayment.DeleteAsync(id);
            //update bill balance
            var studentBills =
                await _repositoryStudentBill.GetAllListAsync(x => x.StudentId == receipt.StudentId && x.BillBalance > 0);

            foreach (var bill in studentBills)
            {
                var amtPaid = receipt.AmountPaid;

                if (bill.BillBalance > amtPaid)
                {
                    bill.BillBalance += receipt.AmountPaid;
                    await _repositoryStudentBill.UpdateAsync(bill);
                }
                else
                {
                    amtPaid = receipt.AmountPaid - bill.BillBalance;
                    bill.BillBalance = 0;
                    await _repositoryStudentBill.UpdateAsync(bill);

                }
            }
        }

        public async Task PostPayments(IEnumerable<Guid> Ids)
        {
            foreach (Guid id in Ids)
            {
                var payment = await _repositoryBillPayment.FirstOrDefaultAsync(x => x.Id == id);
                payment.IsPosted = true;

                await _repositoryBillPayment.UpdateAsync(payment);
            }
        }
        public async Task<IEnumerable<object>> ListAllPosted(Guid academicYearId, Guid termId, Guid classId)
        {
            return await _repositoryBillPayment.GetAllListAsync(a => a.AcademicYearId == academicYearId && a.TermId == termId && a.ClassId == classId && a.IsPosted);

        }
        
        //api support for self service
        
        public async Task<object> PayFees(string studentId, decimal amount, int tenantId)
        {
           
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var studentInfo = await _repositoryStudent.FirstOrDefaultAsync(x => x.StudentIdentifier.Trim() == studentId.Trim());
                

                if (studentInfo == null)
                {
                    return null;
                }
                
                var bill = await _repositoryStudentBill.FirstOrDefaultAsync(x => x.StudentId == studentInfo.Id && x.BillBalance > 0);
               
                var schoolProfile = await (await _repositorySchoolProfile.GetAllAsync()).FirstOrDefaultAsync();
                
                var systemSetting = await (await _repositorySystemSetting.GetAllAsync()).FirstOrDefaultAsync();
                
                var currencyInfo =  await _repositoryCurrency.FirstOrDefaultAsync(systemSetting.DefaultCurrencyId);
                
                if (bill != null)
                {
                    
                    var basicInfo = await _academicPeriodMetaDataManager.GetAcademicPeriodMeta(schoolProfile.CurrentAcademicYearId, schoolProfile.CurrentTermId, studentInfo.ClassId, studentInfo.Id);

                    var paymentId = Guid.NewGuid();
                    
                    var payment = new BillPayment
                    {
                        Id = paymentId,
                        AcademicYearId = schoolProfile?.CurrentAcademicYearId ?? bill.AcademicYearId,
                        TermId = schoolProfile?.CurrentTermId ?? bill.TermId,
                        ClassId = studentInfo.ClassId,
                        StudentId = studentInfo.Id,
                        AmountPaid = amount,
                        PaymentDate = DateTime.UtcNow,
                        CurrencyId = systemSetting.DefaultCurrencyId,
                        CurrencyName = currencyInfo != null ? currencyInfo.Name : "Ghana Cedi",
                        CurrencyCode = currencyInfo != null ? currencyInfo.Code : "GHS",
                        ModeOfPayment = "Mobile Money/Bank Card",
                        BillId = bill.Id,
                        BillNo = bill.BillNo,
                        ReceiptNo = $"{DateTime.UtcNow:yyyyMMddhhmmss}",
                        PaymentDescription = "Fees Payment",
                        IsCreditMemo = false,
                        IsPosted = true,
                        TenantId = tenantId,
                    };
                    
                     
                    payment.CurrencyName = currencyInfo != null ? currencyInfo.Name : "Ghana Cedi";
                    payment.CurrencyCode = currencyInfo != null ? currencyInfo.Code : "GHS";
                    payment.CurrencySellRate = currencyInfo?.SellRate ?? 1 ;
                    payment.CurrencyBuyRate = currencyInfo?.BuyRate ?? 1 ;
                    payment.StudentIdentifier = basicInfo.StudentIdentifier;
                    payment.StudentName = basicInfo.StudentName;
                    payment.TermName = basicInfo.TermName;
                    payment.ClassName = basicInfo.ClassName;
                    payment.AcademicYearName = basicInfo.AcademicYearName;
                    
                    
                     //create statement record...
                
                var currentBill =
                    await _repositoryStudentBill.FirstOrDefaultAsync(x => x.StudentId == payment.StudentId && x.Id == payment.BillId);

                
                var currentStatement =
                    await _repositoryStudentStatement.FirstOrDefaultAsync(a => a.StudentId == payment.StudentId);

                var statementRecord = new List<Statement>
                {
                    new Statement
                    {
                        Id = paymentId,
                        ActivityDate = payment.PaymentDate.ToString("dd-MMM-yyyy"),
                        ReferenceNo = payment.ReceiptNo,
                        Description = payment.PaymentDescription,
                        Invoice = 0,
                        Payment = payment.AmountPaid,
                        Balance = currentBill.BillBalance - payment.AmountPaid,
                        CurrencyName = payment.CurrencyCode,
                        TransactionType = 2,
                        StudentIdentifier = basicInfo.StudentIdentifier,
                        StudentName = basicInfo.StudentName,
                        StudentId = payment.StudentId,

                    }
                };


                if (currentStatement != null)
                {
                    var statementList = currentStatement.Statement;
                    statementList.AddRange(statementRecord);

                    await _repositoryStudentStatement.UpdateAsync(currentStatement);
                }
                else
                {
                    await _repositoryStudentStatement.InsertAsync(new StudentStatement
                    {
                        StudentId = payment.StudentId,
                        Statement = statementRecord,
                        StudentIdentifier= basicInfo.StudentIdentifier,
                        StudentName= basicInfo.StudentName,
                        TenantId = payment.TenantId,

                    });
                }

                //create receipt...

                var receiptRecord = await _repositoryStudentBillReceipt.FirstOrDefaultAsync(a => a.AcademicYearId == payment.AcademicYearId && a.TermId == payment.TermId && a.ClassId == payment.ClassId && a.ReceiptNo == payment.ReceiptNo);
                if (receiptRecord != null)
                {
                    receiptRecord.AmountPaid += payment.AmountPaid;
                    receiptRecord.BalanceDue += (currentBill.BillBalance - payment.AmountPaid);
                }
                else
                {
                    var receipt = new StudentBillReceipt
                    {
                        ReceivedFrom = basicInfo.StudentName,
                        AmountInWords = $"{new UtilService().ConvertToWords(payment.AmountPaid.ToString(CultureInfo.InvariantCulture))}Ghana Cedis Only",
                        AmountPaid = payment.AmountPaid,
                        Reason = payment.PaymentDescription,
                        BalanceDue = currentBill.BillBalance - payment.AmountPaid,
                        PaymentType = payment.ModeOfPayment,
                        StudentId = payment.StudentId,
                        StudentIdentifier = basicInfo.StudentIdentifier,
                        AcademicYearName = basicInfo.AcademicYearName,
                        TermName = basicInfo.TermName,
                        ReceiptNo = payment.ReceiptNo,
                        ClassName = basicInfo.ClassName,
                        PaymentId = paymentId,
                        AcademicYearId = payment.AcademicYearId,
                        TermId = payment.TermId,
                        ClassId = payment.ClassId,
                        TenantId = tenantId,
                    };

                    await _repositoryStudentBillReceipt.InsertAsync(receipt);
                      
                }

                
                    bill.BillBalance = currentBill.BillBalance - payment.AmountPaid;
                    bill.BillStatus = 401;
                    await _repositoryStudentBill.UpdateAsync(bill);
                    
                    await _repositoryBillPayment.InsertAsync(payment);

                    return payment;

                }

                return null;
            }
            
             
        }

       //bill payment by bill type
       
       public async Task<object> PayFees(string studentId, string billType, decimal amount, int tenantId)
        {
           
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var studentInfo = await _repositoryStudent.FirstOrDefaultAsync(x => x.StudentIdentifier.Trim() == studentId.Trim());
                

                if (studentInfo == null)
                {
                    return null;
                }
                
                string normalizedBillType = billType.ToLower().Replace(" ", "");
                
                var bill = await _repositoryStudentBill.FirstOrDefaultAsync(x => x.StudentId == studentInfo.Id && x.BillTypeName.ToLower().Replace(" ", "") == normalizedBillType && x.BillBalance > 0);
               
                var schoolProfile = await (await _repositorySchoolProfile.GetAllAsync()).FirstOrDefaultAsync();
                
                var systemSetting = await (await _repositorySystemSetting.GetAllAsync()).FirstOrDefaultAsync();
                
                var currencyInfo =  await _repositoryCurrency.FirstOrDefaultAsync(systemSetting.DefaultCurrencyId);
                
                if (bill != null)
                {
                    
                    var basicInfo = await _academicPeriodMetaDataManager.GetAcademicPeriodMeta(schoolProfile.CurrentAcademicYearId, schoolProfile.CurrentTermId, studentInfo.ClassId, studentInfo.Id);

                    var paymentId = Guid.NewGuid();
                    
                    var payment = new BillPayment
                    {
                        Id = paymentId,
                        AcademicYearId = schoolProfile?.CurrentAcademicYearId ?? bill.AcademicYearId,
                        TermId = schoolProfile?.CurrentTermId ?? bill.TermId,
                        ClassId = studentInfo.ClassId,
                        StudentId = studentInfo.Id,
                        AmountPaid = amount,
                        PaymentDate = DateTime.UtcNow,
                        CurrencyId = systemSetting.DefaultCurrencyId,
                        CurrencyName = currencyInfo != null ? currencyInfo.Name : "Ghana Cedi",
                        CurrencyCode = currencyInfo != null ? currencyInfo.Code : "GHS",
                        ModeOfPayment = "Mobile Money/Bank Card",
                        BillId = bill.Id,
                        BillNo = bill.BillNo,
                        ReceiptNo = $"{DateTime.UtcNow:yyyyMMddhhmmss}",
                        PaymentDescription = "Fees Payment",
                        IsCreditMemo = false,
                        IsPosted = true,
                        TenantId = tenantId,
                    };
                    
                     
                    payment.CurrencyName = currencyInfo != null ? currencyInfo.Name : "Ghana Cedi";
                    payment.CurrencyCode = currencyInfo != null ? currencyInfo.Code : "GHS";
                    payment.CurrencySellRate = currencyInfo?.SellRate ?? 1 ;
                    payment.CurrencyBuyRate = currencyInfo?.BuyRate ?? 1 ;
                    payment.StudentIdentifier = basicInfo.StudentIdentifier;
                    payment.StudentName = basicInfo.StudentName;
                    payment.TermName = basicInfo.TermName;
                    payment.ClassName = basicInfo.ClassName;
                    payment.AcademicYearName = basicInfo.AcademicYearName;
                    
                    
                     //create statement record...
                
                var currentBill =
                    await _repositoryStudentBill.FirstOrDefaultAsync(x => x.StudentId == payment.StudentId && x.Id == payment.BillId);

                
                var currentStatement =
                    await _repositoryStudentStatement.FirstOrDefaultAsync(a => a.StudentId == payment.StudentId);

                var statementRecord = new List<Statement>
                {
                    new Statement
                    {
                        Id = paymentId,
                        ActivityDate = payment.PaymentDate.ToString("dd-MMM-yyyy"),
                        ReferenceNo = payment.ReceiptNo,
                        Description = payment.PaymentDescription,
                        Invoice = 0,
                        Payment = payment.AmountPaid,
                        Balance = currentBill.BillBalance - payment.AmountPaid,
                        CurrencyName = payment.CurrencyCode,
                        TransactionType = 2,
                        StudentIdentifier = basicInfo.StudentIdentifier,
                        StudentName = basicInfo.StudentName,
                        StudentId = payment.StudentId,
                    }
                };


                if (currentStatement != null)
                {
                    var statementList = currentStatement.Statement;
                    statementList.AddRange(statementRecord);

                    await _repositoryStudentStatement.UpdateAsync(currentStatement);
                }
                else
                {
                    await _repositoryStudentStatement.InsertAsync(new StudentStatement
                    {
                        StudentId = payment.StudentId,
                        Statement = statementRecord,
                        StudentIdentifier= basicInfo.StudentIdentifier,
                        StudentName= basicInfo.StudentName,
                        TenantId = payment.TenantId,

                    });
                }

                //create receipt...

                var receiptRecord = await _repositoryStudentBillReceipt.FirstOrDefaultAsync(a => a.AcademicYearId == payment.AcademicYearId && a.TermId == payment.TermId && a.ClassId == payment.ClassId && a.ReceiptNo == payment.ReceiptNo);
                if (receiptRecord != null)
                {
                    receiptRecord.AmountPaid += payment.AmountPaid;
                    receiptRecord.BalanceDue += (currentBill.BillBalance - payment.AmountPaid);
                }
                else
                {
                    var receipt = new StudentBillReceipt
                    {
                        ReceivedFrom = basicInfo.StudentName,
                        AmountInWords = $"{new UtilService().ConvertToWords(payment.AmountPaid.ToString(CultureInfo.InvariantCulture))}Ghana Cedis Only",
                        AmountPaid = payment.AmountPaid,
                        Reason = payment.PaymentDescription,
                        BalanceDue = currentBill.BillBalance - payment.AmountPaid,
                        PaymentType = payment.ModeOfPayment,
                        StudentId = payment.StudentId,
                        StudentIdentifier = basicInfo.StudentIdentifier,
                        AcademicYearName = basicInfo.AcademicYearName,
                        TermName = basicInfo.TermName,
                        ReceiptNo = payment.ReceiptNo,
                        ClassName = basicInfo.ClassName,
                        PaymentId = paymentId,
                        AcademicYearId = payment.AcademicYearId,
                        TermId = payment.TermId,
                        ClassId = payment.ClassId,
                        TenantId = tenantId,
                    };

                    await _repositoryStudentBillReceipt.InsertAsync(receipt);
                      
                }

                
                    bill.BillBalance = currentBill.BillBalance - payment.AmountPaid;
                    bill.BillStatus = 401;
                    await _repositoryStudentBill.UpdateAsync(bill);
                    
                    await _repositoryBillPayment.InsertAsync(payment);

                    return payment;

                }

                return null;
            }
            
             
        }
    
}