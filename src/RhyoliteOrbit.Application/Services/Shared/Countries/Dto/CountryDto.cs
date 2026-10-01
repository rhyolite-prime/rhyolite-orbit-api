using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.Services.Shared.Countries.Dto;

public class CountryDto
{
    public string Name { get; set; }
    public string IsoCode { get; set; }
    public int NumericIsoCode { get; set; }
    public string Nationality { get; set; }
}

public class RegionDto
{
    public Guid CountryId { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Code { get; set; } = string.Empty;

    public List<City> Cities { get; set; } = new();
}