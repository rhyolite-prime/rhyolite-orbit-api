using System;

namespace RhyoliteOrbit.DomainServices.Pos.Orders;

public class RefundRequestDto
{
    public Guid ItemId { get; set; }
    public int QuantityToRefund { get; set; }
}