using System.Collections.Generic;

namespace RhyoliteOrbit.DomainServices.Subscription.Dto;

public class SubscriptionPaymentDto
{
    public string Combo { get; set; }
    public string PaymentMethod { get; set; }
    public string Cycle { get; set; }
    public decimal Amount { get; set; }
    public bool IsDirectDebitPayment { get; set; }
    public List<int> Modules { get; set; }
    
    //direct debit related field
    public string PhoneNumber { get; set; }
    public string Channel { get; set; }
    public int TenantId { get; set; }
}