namespace RhyoliteOrbit.Models.Shared;

public class City
{
    public string Name { get; set; } = string.Empty;

    public string PostalCode { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }
}