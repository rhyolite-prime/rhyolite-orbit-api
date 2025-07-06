using System;
using System.Net.Http;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Akka.Actor;
using Microsoft.Extensions.Configuration;
using RhyoliteOrbit.Actors;
using RhyoliteOrbit.Actors.MessageModels;
using RhyoliteOrbit.Redis;
using RhyoliteOrbit.SharedDto;
using RhyoliteOrbit.Utils;

namespace RhyoliteOrbit.Mfa;

public class MfaManager: DomainService, IMfaManager
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IRedisCacheManager _redisCacheManager;
    private readonly IActorRef _emailActorProvider;
    private string _mailHostName;
    private int _portNumber;
    private string _password;
    private string _emailAddress;
    private string _defaultDisplayName;

    public MfaManager(IUnitOfWorkManager unitOfWorkManager,IConfiguration configuration, TopLevelActors.EmailActorProvider emailActorProvider, IRedisCacheManager redisCacheManager)
    {
        _unitOfWorkManager = unitOfWorkManager;
        _redisCacheManager = redisCacheManager;
        _emailActorProvider = emailActorProvider();
        _mailHostName = configuration["SmtpCredentials:MailHost"];
        _portNumber = int.Parse(configuration["SmtpCredentials:PortNumber"]);
        _password = configuration["SmtpCredentials:Password"];
        _emailAddress = configuration["SmtpCredentials:EmailAddress"];
        _defaultDisplayName = configuration["SmtpCredentials:DefaultDisplayName"];
    }

    public async Task<object> SendEmailOtp(string email, string subject, int tenantId)
    {
        
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            
            var otp = new Otp().GenerateOtp();
            await _redisCacheManager.SetValueAsync(email.ToLower(), otp);
            await _redisCacheManager.SetExpireTimeAsync(email.ToLower(), TimeSpan.FromMinutes(5));

            
            var emailBody = $@"
                <html>
                <head>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            line-height: 1.6;
                        }}
                        .container {{
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                            border: 1px solid #ddd;
                            border-radius: 5px;
                            background-color: #f9f9f9;
                        }}
                        .header {{
                            text-align: center;
                            padding-bottom: 20px;
                        }}
                        .content {{
                            margin-bottom: 20px;
                        }}
                        .footer {{
                            text-align: center;
                            font-size: 12px;
                            color: #777;
                        }}
                    </style>
                </head>
                <body>
                <div class='container'>
                    <div class='header'>
                        <h2>{subject}</h2>
                    </div>
                    <div class='content'>
                        <p>WARNING! Do NOT share this code with anyone.</p>
                        <p>Your OTP code is <strong>{otp}</strong>.</p>
                    </div>
                    <div class='footer'>
                        <p>&copy; {DateTime.Now.Year} {_defaultDisplayName}. All rights reserved.</p>
                    </div>
                </div>
            </body>
            </html>";
            
            
            _emailActorProvider.Tell(new EmailNotification
            {
                To = email,
                Subject = subject,
                Body = emailBody, 
                Host = _mailHostName,
                Password = _password,
                Port = _portNumber,
                EnableSsl = false, 
                UserName = _emailAddress,
                SenderName = _defaultDisplayName,
                IsBodyHtml = true,
            });
            
            return new
            {
                message = "An OTP has been sent to your email.\nPlease enter the code to continue",
                code = 200,
                otp
            };
            
        }

         
    }


    public async Task<object> SendSmsOtp(string phoneNumber, int tenantId)
    {
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            var otp = new Otp().GenerateOtp();
            await _redisCacheManager.SetValueAsync(phoneNumber.Trim(), otp);
            await _redisCacheManager.SetExpireTimeAsync(phoneNumber.Trim(), TimeSpan.FromMinutes(5));
            
            var smsMessage = $"WARNING! Do NOT share this code with anyone. Your OTP code is {otp}.";
            
            //send sms
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://smsc.hubtel.com/v1/messages/send?clientsecret=bisishci&clientid=xhxykvjj&from=Rhyolite&to={phoneNumber}&content={smsMessage}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var smsNotificationResponse = await response.Content.ReadAsStringAsync();
           
            Logger.Info($"smsNotificationResponse => {smsNotificationResponse}");
            
            return new
            {
                message = "An OTP has been sent to your phone number.\nPlease enter the code to continue",
                code = 200,
                otp
            };
        }
    }
    
    public async Task<DataResponse> VerifyOtp(string identifier, string otp, int tenantId)
    {
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            var otpFromCache = await _redisCacheManager.GetValueAsync( identifier.ToLower(),true);
            
            if (otpFromCache == otp)
            {
                return new DataResponse
                {
                    Message = "OTP verification successful",
                    Code = 200,
                    Success = true
                };
            }
            
            return new DataResponse
            {
                Message = "Invalid OTP",
                Code = 400,
                Success = false
            };
        }
    }
}