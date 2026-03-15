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
        _mailHostName = configuration["AuthSmtpCredentials:MailHost"];
        _portNumber = int.Parse(configuration["AuthSmtpCredentials:PortNumber"]);
        _password = configuration["AuthSmtpCredentials:Password"];
        _emailAddress = configuration["AuthSmtpCredentials:EmailAddress"];
        _defaultDisplayName = configuration["AuthSmtpCredentials:DefaultDisplayName"];
    }

    public async Task<object> SendEmailOtp(string email, string subject, int tenantId)
    {
        
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            
            var otp = new Otp().GenerateOtp();
            await _redisCacheManager.SetValueAsync(email.ToLower(), otp);
            await _redisCacheManager.SetExpireTimeAsync(email.ToLower(), TimeSpan.FromMinutes(5));
            
            var emailBody = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>OTP Verification</title>
            </head>
            <body style='font-family: Arial, sans-serif; background-color: #f3f4f6; margin: 0; padding: 0;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <!-- Email Container -->
                    <div style='background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05); overflow: hidden;'>
                        <!-- Logo Header -->
                        <div style='background-color: #8A0F4D; padding: 5px; text-align: center;'>
                            <img src='https://res.cloudinary.com/rhyoliteprime/image/upload/v1751798440/rhyolite/rhyolite-logo-white.png' alt='RhyolitePrime Logo' style='height: 102px;'>
                        </div>

                        <!-- Content Section -->
                        <div style='padding: 30px;'>
                            <h1 style='font-size: 24px; font-weight: 600; color: #111827; margin-bottom: 20px;'>Your One-Time Password</h1>
                            
                            <p style='color: #4b5563; margin-bottom: 16px;'>
                                Please use the following verification code to complete your login:
                            </p>

                            <!-- OTP Display Box -->
                            <div style='background-color: #f9fafb; border: 1px dashed #d1d5db; border-radius: 6px; padding: 16px; text-align: center; margin: 24px 0;'>
                                <span style='font-size: 28px; font-weight: 700; letter-spacing: 2px; color: #111827;'>{otp}</span>
                            </div>

                            <!-- Warning Text -->
                            <p style='color: #ef4444; font-weight: 500; margin-bottom: 24px;'>
                                ⚠️ Do NOT share this code with anyone.
                            </p>

                            <p style='color: #4b5563; margin-bottom: 8px;'>
                                This code will expire in 5 minutes.
                            </p>
                        </div>

                        <!-- Footer -->
                        <div style='background-color: #f9fafb; padding: 20px; text-align: center; border-top: 1px solid #e5e7eb;'>
                            <p style='color: #6b7280; font-size: 12px; margin: 0;'>
                                © {DateTime.Now.Year} RhyolitePrime. All rights reserved.
                            </p>
                            <p style='color: #6b7280; font-size: 12px; margin: 8px 0 0;'>
                                If you didn't request this, please ignore this email.
                            </p>
                        </div>
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
                EnableSsl = true, 
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
                Message = "Invalid or Expired OTP",
                Code = 400,
                Success = false
            };
        }
    }
}