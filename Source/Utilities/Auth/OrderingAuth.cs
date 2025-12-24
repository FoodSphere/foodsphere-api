using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using FoodSphere.Services;
using FoodSphere.Data.Models;

namespace FoodSphere.Configurations
{
    public static class JwtOrderingConfiguration
    {
        public const string SchemeName = nameof(JwtOrderingConfiguration);

        public static AuthenticationBuilder AddJwtOrdering(
            this AuthenticationBuilder builder,
            ICredentialService credentialService
        ) {
            return builder.AddJwtBearer(SchemeName, Configure(credentialService));
        }

        public static Action<JwtBearerOptions> Configure(ICredentialService credentialService)
        {
            return options => {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = credentialService["Domain:api:url"],
                    ValidAudience = credentialService["Domain:ordering:url"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = credentialService.GetSecurityKey("Domain:ordering:signing_key")
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
            var principal = context.Principal!;

            var billId = principal.FindFirstValue(AppClaimType.BillClaimType);
            var billMemberId = principal.FindFirstValue(AppClaimType.BillMemberClaimType);

            if (billId is null || billMemberId is null)
            {
                context.Fail("invalid claims");
                return;
            }

            var billService = context.HttpContext.RequestServices.GetRequiredService<BillService>();

            var billMember = await billService.GetBillMember(Guid.Parse(billId), short.Parse(billMemberId));

            if (billMember is null)
            {
                context.Fail("bill member not found.");
                return;
            }

            context.HttpContext.Items[nameof(BillMember)] = billMember;
        }

        public static async Task OnAuthenticationFailed(AuthenticationFailedContext context)
        {
        }
    }
}

namespace FoodSphere.Services
{
    public class OrderingAuthService(
        AppDbContext context,
        ICredentialService credentialService
    ) : BaseService(context)
    {
        readonly ICredentialService _credentialService = credentialService;

        public async Task<ClaimsIdentity> GetSubject(BillMember billMember)
        {
            List<Claim> claims = [];

            var consumerId = billMember.ConsumerId;

            if (consumerId is not null)
            {
                claims.Add(new(AppClaimType.Identity.UserIdClaimType, consumerId.ToString()!));
            }

            return new ClaimsIdentity(claims);
        }

        public async Task<Dictionary<string, object>> GetClaims(BillMember billMember)
        {
            var claims = new Dictionary<string, object>
            {
                [AppClaimType.BillClaimType] = billMember.BillId,
                [AppClaimType.BillMemberClaimType] = (int)billMember.Id // or use .ToString()
            };

            return claims;
        }

        public async Task<SecurityTokenDescriptor> GetTokenDescriptor(BillMember billMember)
        {
            // make expire
            return new SecurityTokenDescriptor
            {
                Issuer = _credentialService["Domain:api:url"],
                Audience = _credentialService["Domain:ordering:url"],
                Subject = await GetSubject(billMember),
                Claims = await GetClaims(billMember),
                Expires = DateTime.UtcNow.AddMinutes(300),
                SigningCredentials = _credentialService.SigningCredentials("Domain:ordering:signing_key"),
            };
        }

        public async Task<string> GenerateToken(BillMember billMember)
        {
            var handler = new JsonWebTokenHandler();
            var token = handler.CreateToken(await GetTokenDescriptor(billMember));

            return token;
        }
    }
}