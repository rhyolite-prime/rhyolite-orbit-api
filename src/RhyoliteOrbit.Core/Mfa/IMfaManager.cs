using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.SharedDto;

namespace RhyoliteOrbit.Mfa;

public interface IMfaManager: IDomainService
{
    Task<object> SendEmailOtp(string email, string subject, int tenantId);
    Task<object> SendSmsOtp(string phoneNumber, int tenantId);
    Task<DataResponse> VerifyOtp(string identifier, string otp, int tenantId);
}