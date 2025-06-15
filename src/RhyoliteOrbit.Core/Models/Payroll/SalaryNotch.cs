using System;

namespace RhyoliteOrbit.Models.Payroll;

public class SalaryNotch
{
    public Guid Id { get; set; }
    public int NumericalOrder { get; set; }
    public Guid SalaryGradeId { get; set; }
    public string SalaryGradeName { get; set; }
    public string Notch { get; set; }
    public decimal Salary { get; set; }
}