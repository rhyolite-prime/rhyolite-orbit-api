using System;

namespace RhyoliteOrbit.Services.Pos.CustomerRanks.Dto;

public class CustomerRankDto
{
    public string Name { get; set; }
    public decimal MinimumTotalSpend { get; set; }
    public decimal InStoreDiscount { get; set; }
    public decimal OnlineDiscount { get; set; }
    public Guid CurrencyId { get; set; }
    public string CurrencyCode { get; set; }
}