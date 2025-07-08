using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.DomainServices.Payroll.Reports.Dto;

namespace RhyoliteOrbit.Pdf.Payslip;

public interface IPayslipPdfGenerator: IDomainService
{
    Task<byte[]> RenderPdf(EmployeePayslip payslip);
}