using System;

namespace RhyoliteOrbit.DomainServices.Payroll.BikTypes.Dto;

public class BikRateDto
{
    public Guid BikTypeId { get; set; }
    public Guid Id { get; set; }
    public Guid EmployeeCategoryId { get; set; }
    public string BikTypeName { get; set; }
    public string EmployeeCategoryName { get; set; }
    public Guid AllowanceTypeId { get; set; }
    public string AllowanceTypeName { get; set; }
    public bool FixedAmount { get; set; }
    public decimal Amount { get; set; }
    public decimal PercentageBasic { get; set; }
    public decimal MaximumAmount { get; set; }
}