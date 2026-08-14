using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Pos;

public class NotificationAccount: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    //plugins will make use of notifications 
    
    [MaxLength(100)]
    public string Alias { get; set; }
        
    [Column(TypeName = "jsonb")] public SmtpCredential SmtpCredentials { get; set; }
    
    [Column(TypeName = "jsonb")] public TelegramApiCredential TelegramApiCredentials { get; set; }
    
    [Column(TypeName = "jsonb")] public SmsApiCredential SmsApiCredentials { get; set; }
    
    [Column(TypeName = "jsonb")] public WhatsAppApiCredential WhatsAppApiCredentials { get; set; }
    
    public bool IsEmailAccountActive { get; set; }
    
    public bool IsTelegramAccountActive { get; set; }
    
    public bool IsWhatsAppAccountActive { get; set; }
    
    public bool IsSmsAccountActive { get; set; }
    
    public int TenantId { get; set; }
}