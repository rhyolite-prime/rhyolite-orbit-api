using System;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class FinishedGoodAllocation: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid FinishedGoodId { get; set; }

    public Guid CustomerId { get; set; } // from shared resource
    public string CustomerName { get; set; }

    // user selects customer and the view displays all invoices of the customer
    // user then tags the invoice to the finished good
    
    public decimal QuantityAllocated { get; set; }
    
    public Guid ArInvoiceId { get; set; }
    public string ArInvoiceNumber { get; set; }
    
    public int TenantId { get; set; }
}