using System;

namespace RhyoliteOrbit.DomainServices.Payroll.SalaryGrades.Dto;

public class SalaryNotchDto
{
    public Guid Id { get; set; }
    public Guid SalaryGradeId { get; set; }
    public string SalaryGradeName { get; set; }
    public string Notch { get; set; }
    public int NumericalOrder { get; set; }
        
    public decimal Salary { get; set; }
}