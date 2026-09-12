using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.Services.Pos.NotificationAccounts.Dto;

public class NotificationAccountDto
{
    public string Alias { get; set; }
        
    public SmtpCredential SmtpCredentials { get; set; }
    
    public TelegramApiCredential TelegramApiCredentials { get; set; }
    
    public SmsApiCredential SmsApiCredentials { get; set; }
    
    public WhatsAppApiCredential WhatsAppApiCredentials { get; set; }
    
    public bool IsEmailAccountActive { get; set; }
    
    public bool IsTelegramAccountActive { get; set; }
    
    public bool IsWhatsAppAccountActive { get; set; }
    
    public bool IsSmsAccountActive { get; set; }
}