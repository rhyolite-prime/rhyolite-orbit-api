using System;

namespace RhyoliteOrbit.DomainServices.Payroll.DeductionTypes.Dto;

public class DeductionRateDto
{
    public Guid Id { get; set; }
    public Guid DeductionTypeId { get; set; }
    public Guid EmployeeCategoryId { get; set; }
    public bool Prorate { get; set; }
    public bool FixedAmount { get; set; }
    public decimal Amount { get; set; }
    public decimal MaximumAmount { get; set; }
    public decimal PercentageBasic { get; set; }
}