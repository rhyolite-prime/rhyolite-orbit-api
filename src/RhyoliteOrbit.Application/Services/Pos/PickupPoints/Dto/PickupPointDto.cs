namespace RhyoliteOrbit.Services.Pos.PickupPoints.Dto;

public class PickupPointDto
{
    public string FriendlyName { get; set; }
    public string SystemName { get; set; }
    public string ImageUrl { get; set; }
    public string GhanaPostGps { get; set; }
    public string GpsCoordinates { get; set; }
    public int IsActive { get; set; }
}