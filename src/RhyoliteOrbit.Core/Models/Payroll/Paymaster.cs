using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Payroll;

public class Paymaster: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
        public Guid EmployeeId { get; set; }
        [MaxLength(20)]
        public string EmployeeIdentifier { get; set; }
        [MaxLength(130)]
        public string EmployeeName { get; set; }
        [MaxLength(100)]
        public string EmployeeDepartment { get; set; }
        [MaxLength(100)]
        public string EmployeeCategory { get; set; }
        public decimal EmployeeSsfDeduction { get; set; }
        public decimal EmployerSsfDeduction { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal IrsTax { get; set; }
        public decimal NetSalary { get; set; }
        public decimal TaxableAllowance { get; set; }
        public decimal NonTaxableAllowance { get; set; }
        public decimal TaxableOverTime { get; set; }
        public decimal OverTimeTax { get; set; }
        public decimal NonTaxableOverTime { get; set; }
        public decimal VoluntaryDeduction { get; set; }
        public decimal DaysWorked { get; set; }
        public decimal TaxRelief { get; set; }
        public decimal BenefitsInKind { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal EmployeeProvidentFundContribution { get; set; }
        public decimal EmployerProvidentFundContribution { get; set; }
        public decimal EmployeeSecProvidentFundContribution { get; set; }
        public decimal EmployerSecProvidentFundContribution { get; set; }
        public decimal TaxableSpecialEarning { get; set; }
        public decimal NonTaxableSpecialEarning { get; set; }
        public decimal OneTimeDeduction { get; set; }
        public decimal SpecialEarning { get; set; }
        public decimal BonuxTax { get; set; }
        public bool IsPaid { get; set; }
        public Guid CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public decimal BuyRate { get; set; }
        public decimal SellRate { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public bool WithHeld { get; set; }
        public decimal WithHeldRate { get; set; }
        public decimal OvertimeExcessTax { get; set; }
        public string ContributionType { get; set; }
        public decimal EmployeeContribution { get; set; }
        public decimal EmployerContribution { get; set; }
        public decimal LoanDeduction { get; set; }
        public decimal TaxableIncome { get; set; }
        public decimal TotalSalaryAdvance { get; set; }
        public int TenantId { get; set; }
}