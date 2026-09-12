using System;

namespace RhyoliteOrbit.Services.Pos.ShopCarts.Dto;

public class ShopCartDto
{
    public Guid ItemId { get; set; }
    public int Quantity { get; set; } = 0;
}