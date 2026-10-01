using System;
using System.Text.Json;

namespace RhyoliteOrbit.Services.Shared.PaymentOptions.Dto;

public class PaymentOptionDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsDefault { get; set; }
    public JsonDocument Exec { get; set; }
    public JsonDocument MetaData { get; set; }
}