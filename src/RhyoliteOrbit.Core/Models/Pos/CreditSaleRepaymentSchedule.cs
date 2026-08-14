using System;

namespace RhyoliteOrbit.Models.Pos;

public class CreditSaleRepaymentSchedule: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid CreditSaleId { get; set; }
    public decimal MonthlyPayment { get; set; }
    public decimal PrincipalPayment { get; set; }
    public decimal InterestPayment { get; set; }
    public decimal PrincipalBalance { get; set; }
    public decimal InterestPlusPrincipalBalance { get; set; }
    public bool IsPaid { get; set; }
    public DateTime DueDate { get; set; }
    public int Period { get; set; }
    public int TenantId { get; set; }
}