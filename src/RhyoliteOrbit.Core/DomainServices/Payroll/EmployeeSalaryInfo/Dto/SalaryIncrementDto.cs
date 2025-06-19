using System;
using System.Collections.Generic;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeSalaryInfo.Dto;

public class SalaryIncrementDto
{
    public List<SalaryIncrement> SalaryData { get; set; }
}

public class SalaryIncrement
{
    public Guid EmployeeId { get; set; }
    public decimal PreviousSalary { get; set; }
    public decimal MonthlySalary { get; set; }
    public decimal IncrementAmount { get; set; }
}