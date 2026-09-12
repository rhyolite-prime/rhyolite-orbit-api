namespace RhyoliteOrbit.Services.Pos.Branches.Dto;

public class PosBranchDto
{
    public string Name { get; set; }
    public string Location { get; set; }
    public string CountryName { get; set; }
    public string CountryCode { get; set; }
    public string RegionOrState { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public string PhoneNo { get; set; }
    public string BranchManager { get; set; }
    public string GeoCoordinates { get; set; }
    public bool IsDefault { get; set; }
}