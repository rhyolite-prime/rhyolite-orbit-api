using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RhyoliteOrbit.DomainServices.SchoolManager.StudentBills.Dto;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.StudentBills;

public interface IStudentBillManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(Guid academicYearId, Guid termId, Guid classId, Guid billTypeId);
    Task<object> ListBillsToCancel(Guid academicYearId, Guid termId, Guid classId, Guid billTypeId);
    
    Task CreateBatchBills(StudentBillBatchDto dto);
    Task<object> ListAllBalances(Guid academicYearId, Guid termId, Guid classId);
    Task<object> GetCurrentBillDetails(Guid academicYearId, Guid termId, Guid classId, Guid billTypeId);
    Task<object> ListOpeningBalancesToCancel(Guid academicYearId, Guid termId, Guid classId);
    Task<object> ListStudentBills(Guid studentId);
    Task<object> GetBill(Guid id);
    
    //enquiry
    Task<object> GetPaidUpStudents(Guid academicYearId, Guid termId, Guid classId);
    Task<object> GetStudentDebtors(Guid academicYearId, Guid termId, Guid classId);
    Task CancelBill(StudentBill bill);
    Task CreateOpeningBalance(StudentBill studentBill);
    //exposed api ...
    Task<object> GetParentWardsBillSummary(string phoneNo, int tenantId);
    Task<object> GetStudentBillSummary(string studentId, string pin, int tenantId);
    Task<object> GetBillSummaryByBillType(string studentId, string pin, string billType, int tenantId);
    Task Delete(Guid id);
}