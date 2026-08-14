using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Pos;

public class RefundAndReturn: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(20)]
    public string RequestNo { get; set; }
    [MaxLength(20)]
    public string ReceiptNo { get; set; }
    public Guid OrderId { get; set; }
    [MaxLength(20)]
    public string CustomerPhoneNo { get; set; }
    [MaxLength(150)]
    public string CustomerName { get; set; }
    public Guid ItemId { get; set; }
    [MaxLength(150)]
    public string ItemName { get; set; }
    public int Quantity { get; set; }
    [MaxLength(250)]
    public string ReasonForReturnOrExchange { get; set; }
    [MaxLength(20)]
    public string RequestedAction { get; set; }
    public int TenantId { get; set; }
}