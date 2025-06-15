using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Shared;

public class SystemSetting: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    //smtp credentials
    [MaxLength(100)]
    public string MailHostName { get; set; }
    public bool IsSslEnabled { get; set; }
    public int PortNumber { get; set; }
    [MaxLength(150)]
    public string PrimaryEmailAddress { get; set; }
    [MaxLength(100)]
    public string EmailPassword { get; set; }
    
    //currency settings
    public Guid DefaultCurrencyId { get; set; }
    
    // Email/SMS notification settings for bill payments: in school manager
    public bool SendBillPaymentEmailNotification { get; set; }
    public bool SendBillPaymentSmsNotification { get; set; }
    [MaxLength(350)]
    public string BillPaymentEmailTemplateHtmlUrl { get; set; }
    [MaxLength(350)]
    public string BillPaymentSmsTemplateHtmlUrl { get; set; }
    [MaxLength(350)]
    public string TerminalReportTemplateHtmlUrl { get; set; }
      
    public int TenantId { get; set; }
}