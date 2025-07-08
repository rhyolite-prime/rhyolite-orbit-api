using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeBio.Dto;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeBio;

public interface IEmployeeBioDataManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> Create(EmployeeBioData input);
    Task<EmployeeBioData> GetAsync(string employeeIdentifier);
    Task Update(EmployeeBioData input);
    Task Delete(Guid id);
    Task UpdateStatus(Guid employeeId, Guid statusId);

    //export
    Task<List<AllowanceExportDto>> ExportAllowances();
    Task<List<BenefitsExportDto>> ExportBenefitsInKind();
    Task<List<SalaryInfoDto>> ExportSalaryInfo();
    Task<List<SsnitExportDto>> ExportSsnit();
    Task<List<DeductionsExportDto>> ExportDeductions();
    Task<List<EmployeeLoansExportDto>> ExportEmployeeLoans();
}