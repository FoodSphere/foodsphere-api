using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace FoodSphere.SelfOrdering.Api.Authentication;

public static class JwtAuthentication
{
    public const string SchemeName = nameof(JwtAuthentication);

    extension(AuthenticationBuilder builder)
    {
        public AuthenticationBuilder AddSelfOrderingJwt(IServiceCollection services)
        {
            using var sp = services.BuildServiceProvider();

            return builder.AddJwtBearer(SchemeName, Configure(sp));
        }
    }

    public static Action<JwtBearerOptions> Configure(IServiceProvider sp)
    {
        var envDomainApi = sp.GetRequiredService<IOptions<EnvDomainApi>>().Value;
        var envDomainOrdering = sp.GetRequiredService<IOptions<EnvDomainOrdering>>().Value;

        return options => {
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = envDomainApi.hostname,
                ValidAudience = envDomainOrdering.hostname,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = envDomainOrdering.GetSecurityKey()
            };
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = OnTokenValidated,
                OnAuthenticationFailed = OnAuthenticationFailed,
            };
        };
    }

    static async Task OnTokenValidated(TokenValidatedContext context)
    {
        var principal = context.Principal!;
        var sp = context.HttpContext.RequestServices;

        var billIdClaim = principal.FindFirstValue(FoodSphereClaimType.BillClaimType);
        var memberIdClaim = principal.FindFirstValue(FoodSphereClaimType.BillMemberClaimType);

        var logger = sp.GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(JwtAuthentication));

        if (!Guid.TryParse(billIdClaim, out var billId) ||
            !short.TryParse(memberIdClaim, out var memberId))
        {
            context.Fail("invalid claims");
            return;
        }

        var billService = sp.GetRequiredService<BillService>();

        var member = billService.GetMemberStub(billId, memberId);

        context.HttpContext.Items[nameof(BillMember)] = member;
    }

    static async Task OnAuthenticationFailed(AuthenticationFailedContext context)
    {
        var sp = context.HttpContext.RequestServices;

        var logger = sp.GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(JwtAuthentication));

        logger.LogWarning("ordering's token failed: {msg}", context.Exception.Message);
    }
}