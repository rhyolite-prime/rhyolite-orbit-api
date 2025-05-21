using System;

namespace RhyoliteOrbit.Models.Shared;

public class SupplierAddress
{
    public Guid Id { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string Country { get; set; }
    public string RegionOrState { get; set; }
    public string CityOrLocation { get; set; }
    public string BranchName { get; set; }
}