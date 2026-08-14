using System.Collections.Generic;

namespace RhyoliteOrbit.Models.Pos;

public class SmtpCredential
{
    public string DisplayName { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public List<string> RecipientList { get; set; }
    public bool EnableSsl { get; set; }
    public string Email { get; set; }
}