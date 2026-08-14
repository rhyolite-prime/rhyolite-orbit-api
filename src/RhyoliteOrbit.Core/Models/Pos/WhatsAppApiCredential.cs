using System.Collections.Generic;

namespace RhyoliteOrbit.Models.Pos;

public class WhatsAppApiCredential
{
    public string WhatsAppNumber { get; set; }
    public List<string> RecipientList { get; set; }
}