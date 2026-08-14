using System.Collections.Generic;

namespace RhyoliteOrbit.Models.Pos;

public class TelegramApiCredential
{
    public string TelegramBotToken { get; set; }
    public List<string> RecipientList { get; set; }
}