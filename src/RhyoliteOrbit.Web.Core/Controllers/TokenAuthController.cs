using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using RhyoliteOrbit.Authentication.JwtBearer;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.Authorization.Users;
using RhyoliteOrbit.Models.TokenAuth;
using RhyoliteOrbit.MultiTenancy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Abp;
using Abp.Domain.Uow;

namespace RhyoliteOrbit.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TokenAuthController : RhyoliteOrbitControllerBase
    {
        private readonly LogInManager _logInManager;
        private readonly ITenantCache _tenantCache;
        private readonly AbpLoginResultTypeHelper _abpLoginResultTypeHelper;
        private readonly TokenAuthConfiguration _configuration;
        private readonly IPermissionManager _permissionManager;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public TokenAuthController(
            LogInManager logInManager,
            ITenantCache tenantCache,
            AbpLoginResultTypeHelper abpLoginResultTypeHelper,
            TokenAuthConfiguration configuration, IPermissionManager permissionManager, IPermissionChecker permissionChecker, IUnitOfWorkManager unitOfWorkManager)
        {
            _logInManager = logInManager;
            _tenantCache = tenantCache;
            _abpLoginResultTypeHelper = abpLoginResultTypeHelper;
            _configuration = configuration;
            _permissionManager = permissionManager;
            _permissionChecker = permissionChecker;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [HttpPost]
        public async Task<AuthenticateResultModel> Authenticate([FromBody] AuthenticateModel model)
        {
            var loginResult = await GetLoginResultAsync(
                model.UserNameOrEmailAddress,
                model.Password,
                model.AccountName
            );

            var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));

            // Get all granted permissions for the logged-in user
            var permissionNames = await GetEffectivePermissionNamesAsync(loginResult);

            return new AuthenticateResultModel
            {
                AccessToken = accessToken,
                EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds,
                UserId = loginResult.User.Id
            };
        }
        
        private async Task<List<string>> GetEffectivePermissionNamesAsync(AbpLoginResult<Tenant, User> loginResult)
        {
            var tenantId = loginResult.User.TenantId;
            var userIdentifier = new UserIdentifier(tenantId, loginResult.User.Id);

            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var grantedPermissionNames = new List<string>();

                foreach (var permission in await _permissionManager.GetAllPermissionsAsync())
                {
                    if (await _permissionChecker.IsGrantedAsync(userIdentifier, permission.Name))
                    {
                        grantedPermissionNames.Add(permission.Name);
                    }
                }

                return grantedPermissionNames;
            }
        }


        private string GetTenancyNameOrNull()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
        }

        private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result, usernameOrEmailAddress, tenancyName);
            }
        }

        private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            var now = DateTime.UtcNow;

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(expiration ?? _configuration.Expiration),
                signingCredentials: _configuration.SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        private static List<Claim> CreateJwtClaims(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == ClaimTypes.NameIdentifier);

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });

            return claims;
        }

        private string GetEncryptedAccessToken(string accessToken)
        {
            return SimpleStringCipher.Instance.Encrypt(accessToken);
        }
    }
}
