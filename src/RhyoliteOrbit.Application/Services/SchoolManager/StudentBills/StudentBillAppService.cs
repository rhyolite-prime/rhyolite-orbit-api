using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.SchoolManager.StudentBills;
using RhyoliteOrbit.DomainServices.SchoolManager.StudentBills.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.StudentBills;

public class StudentBillAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IStudentBillManager _studentBillManager;
    private readonly IValidateTenant _validateTenant;

    public StudentBillAppService(IStudentBillManager studentBillManager, IValidateTenant validateTenant)
    {
        _studentBillManager = studentBillManager;
        _validateTenant = validateTenant;
    }

    [HttpPost]
    public async Task Create(StudentBillBatchDto dto)
    {
        await _studentBillManager.CreateBatchBills(dto);
    }
    
    [HttpPost]
    public async Task UpdateBillWithDocumentUrl(StudentBillReportUpdateDto dto)
    {
        await _studentBillManager.UpdateBillWithDocumentUrl(dto);
    }
    
    
    
    [HttpGet]
    public async Task<object> GetPaidUpStudents(Guid academicYearId, Guid termId, Guid classId)
    {
        return await _studentBillManager.GetPaidUpStudents(academicYearId, termId, classId);
    }
    
    
    [HttpGet]
    public async Task<object> GetStudentBills(Guid studentId)
    {
        return await _studentBillManager.ListStudentBills(studentId);
    }
    
    [HttpGet]
    public async Task<object> GetBillHistory(int pageNo, int pageSize, string query, Guid classId)
    {
        return await _studentBillManager.ListBillHistory(pageNo, pageSize, query, classId);
    }
    
    
    
    [HttpGet]
    public async Task<object> GetStudentDebtors(Guid academicYearId, Guid termId, Guid classId)
    {
        return await _studentBillManager.GetStudentDebtors(academicYearId, termId, classId);
    }
    
    [HttpGet]
    public async Task<object> GetStudentBillSummary(string studentId, string pin)
    {
        var tenantId = await _validateTenant.Validate();
        
        return await _studentBillManager.GetStudentBillSummary(studentId, pin, tenantId);
    }
    
    [HttpGet]
    public async Task<object> GetBillSummaryByBillType(string studentId, string pin, string billType, int tenantId)
    {
        return await _studentBillManager.GetBillSummaryByBillType(studentId, pin, billType, tenantId);
    }
    
    
    [HttpDelete]
    public async Task Delete(Guid id)
    { 
        await _studentBillManager.Delete(id);
    }
    
    
    
    
    
    
    
}