using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Pos;

public class Payment: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid OrderId { get; set; }
    
    public Guid CreditSaleId { get; set; }
    
    public bool IsCreditSale { get; set; }
        
    [MaxLength(100)]
    public string CustomerName { get; set; }
    
    [MaxLength(15)]
    public string CustomerPhoneNo { get; set; }
        
    [MaxLength(15)]
    public string Status { get; set; }
    
    public decimal InvoiceAmount { get; set; }
    
    public decimal AmountPaid { get; set; }
        
    public decimal InterestComponent { get; set; }
        
    public decimal PenaltyComponent { get; set; }
        
    public decimal PrincipalComponent { get; set; }
    
    public decimal Balance { get; set; }
    
    [MaxLength(20)]
    public string ReceiptNo { get; set; }
    
    [MaxLength(20)]
    public string PaymentType { get; set; } //full or installment => for hire purchase
    
    [MaxLength(20)]
    public string PaymentChannel { get; set; } 
         
    public int TenantId { get; set; }
}