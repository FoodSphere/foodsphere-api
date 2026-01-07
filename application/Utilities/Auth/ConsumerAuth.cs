using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using FoodSphere.Data.Models;
using FoodSphere.Services;

namespace FoodSphere.Configurations
{
    public static class JwtConsumerConfiguration
    {
        public const string SchemeName = nameof(JwtConsumerConfiguration);

        public static AuthenticationBuilder AddJwtConsumer(this AuthenticationBuilder builder, ICredentialService credentialService)
        {
            return builder.AddJwtBearer(SchemeName, Configure(credentialService));
        }

        public static Action<JwtBearerOptions> Configure(ICredentialService credentialService)
        {
            return options => {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = credentialService["Domain:api:url"],
                    ValidAudience = credentialService["Domain:consumer:url"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = credentialService.GetSecurityKey("Domain:consumer:signing_key")
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = OnTokenValidated,
                    OnAuthenticationFailed = OnAuthenticationFailed,
                };
            };
        }

        public static async Task OnTokenValidated(TokenValidatedContext context)
        {
        }

        public static async Task OnAuthenticationFailed(AuthenticationFailedContext context)
        {
        }
    }
}

namespace FoodSphere.Services
{
    public class ConsumerAuthService(
        UserManager<MasterUser> userManager,
        ICredentialService credentialService
    ) {
        readonly UserManager<MasterUser> _userManager = userManager;
        readonly ICredentialService _credentialService = credentialService;

        async Task<ClaimsIdentity> GetSubject(ConsumerUser user)
        {
            List<Claim> claims = [
                new(AppClaimType.Identity.UserIdClaimType, user.Id.ToString()),
                // new(AppClaimTypes.Identity.SecurityStampClaimType, user.SecurityStamp ?? string.Empty),
            ];

            return new ClaimsIdentity(claims);
        }

        async Task<Dictionary<string, object>> GetClaims(ConsumerUser user)
        {
            var claims = new Dictionary<string, object>
            {
            };

            return claims;
        }

        async Task<SecurityTokenDescriptor> GetTokenDescriptor(ConsumerUser user)
        {
            return new SecurityTokenDescriptor
            {
                Issuer = _credentialService["Domain:api:url"],
                Audience = _credentialService["Domain:consumer:url"],
                Subject = await GetSubject(user),
                Claims = await GetClaims(user),
                Expires = DateTime.UtcNow.AddMinutes(300),
                SigningCredentials = _credentialService.SigningCredentials("Domain:consumer:signing_key"),
            };
        }

        public async Task<string> GenerateToken(ConsumerUser user)
        {
            var handler = new JsonWebTokenHandler();
            var token = handler.CreateToken(await GetTokenDescriptor(user));

            return token;
        }

        public async Task<bool> IsTwoFactorEnabledAsync(ConsumerUser user)
        {
            return user.TwoFactorEnabled;
        }
    }
}