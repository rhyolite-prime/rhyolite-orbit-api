using System.Collections.Generic;

namespace RhyoliteOrbit.Models.Pos;

public class SmsApiCredential
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public List<string> RecipientList { get; set; }
}