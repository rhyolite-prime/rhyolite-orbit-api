using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Net.Mail;
using RhyoliteOrbit.Actors.MessageModels;

namespace RhyoliteOrbit.Notifications;

public interface ICustomEmailSender: IEmailSender
{
    Task SendEmailWithAttachmentsAsync(
        string to,
        string subject,
        string body,
        List<EmailAttachment> attachments,string host , int port , bool enableSsl , string userName , string password, string defaultFromDisplayName,
        bool isBodyHtml = true);
}