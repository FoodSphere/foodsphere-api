using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace FoodSphere.Consumer.Api.Authentication;

public static class JwtAuthentication
{
    public const string SchemeName = nameof(JwtAuthentication);

    extension(AuthenticationBuilder builder)
    {
        public AuthenticationBuilder AddConsumerJwt(IServiceCollection services)
        {
            using var sp = services.BuildServiceProvider();

            return builder.AddJwtBearer(SchemeName, Configure(sp));
        }
    }

    public static Action<JwtBearerOptions> Configure(IServiceProvider sp)
    {
        var envDomainApi = sp.GetRequiredService<IOptions<EnvDomainApi>>().Value;
        var envDomainConsumer = sp.GetRequiredService<IOptions<EnvDomainConsumer>>().Value;
        var envDomainOrdering = sp.GetRequiredService<IOptions<EnvDomainOrdering>>().Value;

        return options => {
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = envDomainApi.hostname,
                ValidAudience = envDomainConsumer.hostname,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = envDomainConsumer.GetSecurityKey()
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

        var userId = principal.FindFirstValue(FoodSphereClaimType.Identity.UserIdClaimType);

        var logger = sp.GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(JwtAuthentication));

        if (userId is null)
        {
            context.Fail("invalid user_id");
            return;
        }
    }

    static async Task OnAuthenticationFailed(AuthenticationFailedContext context)
    {
        var sp = context.HttpContext.RequestServices;

        var logger = sp.GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(JwtAuthentication));

        logger.LogWarning("consumer's token failed: {msg}", context.Exception.Message);
    }
}