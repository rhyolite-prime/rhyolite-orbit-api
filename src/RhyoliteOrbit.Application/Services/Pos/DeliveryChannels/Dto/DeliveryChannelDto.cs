using System.Collections.Generic;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.Services.Pos.DeliveryChannels.Dto;

public class DeliveryChannelDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsLocal { get; set; }
    public List<DeliveryZoneRate> ZoneRates { get; set; }
}