using System;

namespace RhyoliteOrbit.Models.GeneralLedger;

public class BudgetDetail
{
    public Guid Id { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal BudgetAmount { get; set; }
    public decimal RevisedAmount { get; set; }
}