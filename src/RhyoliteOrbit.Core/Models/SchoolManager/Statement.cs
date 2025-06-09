using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class Statement
{
    public Guid Id { get; set; }
    public string ActivityDate { get; set; }
    public string ReferenceNo { get; set; }
    public string Description { get; set; }
    public decimal Invoice { get; set; }
    public decimal Payment { get; set; }
    public decimal Balance { get; set; }
    public string CurrencyName { get; set; }
    public int TransactionType { get; set; }
    public Guid StudentId { get; set; }
    public string StudentIdentifier { get; set; }
    public string StudentName { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime BillDate { get; set; }
}