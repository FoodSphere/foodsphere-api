using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace FoodSphere.Pos.Api.Authentication;

public static class JwtAuthentication
{
    public const string SchemeName = nameof(JwtAuthentication);

    extension(AuthenticationBuilder builder)
    {
        public AuthenticationBuilder AddPosJwt(IServiceCollection services)
        {
            using var sp = services.BuildServiceProvider();

            return builder.AddJwtBearer(SchemeName, Configure(sp));
        }
    }

    static Action<JwtBearerOptions> Configure(IServiceProvider sp)
    {
        var envDomainApi = sp.GetRequiredService<IOptions<EnvDomainApi>>().Value;
        var envDomainMaster = sp.GetRequiredService<IOptions<EnvDomainMaster>>().Value;
        var envDomainPos = sp.GetRequiredService<IOptions<EnvDomainPos>>().Value;

        return options => {
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = envDomainApi.hostname,
                ValidAudience = envDomainPos.hostname,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = [
                    envDomainMaster.GetSecurityKey(),
                    envDomainPos.GetSecurityKey()
                ]
            };
            options.Events = new JwtBearerEvents
            {
                // signalR client can't send token in header, so we allow it in query string for hub endpoints
                OnMessageReceived = async context =>
                {
                    var accessToken = context.Request.Query["access_token"].FirstOrDefault();
                    var path = context.HttpContext.Request.Path.Value ?? "";

                    if (!string.IsNullOrEmpty(accessToken) && path.Contains("/hubs/"))
                    {
                        context.Token = accessToken;
                    }
                },
                OnTokenValidated = OnTokenValidated,
                OnAuthenticationFailed = OnAuthenticationFailed,
            };
        };
    }

    static async Task ValidateMaster(TokenValidatedContext context)
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

        var userManager = sp.GetRequiredService<UserManager<MasterUser>>();

        // TokenValidationParameters only check signature, expiration
        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            context.Fail("master not found");
            return;
        }

        var securityStamp = principal.FindFirstValue(FoodSphereClaimType.Identity.SecurityStampClaimType);

        // GetSecurityStampAsync ensure fresh value; user.SecurityStamp possibly outdated
        if (securityStamp is null || securityStamp != await userManager.GetSecurityStampAsync(user))
        {
            context.Fail("security stamp validation failed");
            return;
        }

        context.HttpContext.Items[nameof(MasterUserKey)] = (MasterUserKey)user;
        context.HttpContext.Items[nameof(UserType)] = UserType.Master;
        logger.LogInformation("master's token passed");
    }

    static async Task ValidateWorker(TokenValidatedContext context)
    {
        var principal = context.Principal!;
        var sp = context.HttpContext.RequestServices;

        var userId = principal.FindFirstValue(FoodSphereClaimType.Identity.UserIdClaimType);
        var restaurantId = principal.FindFirstValue(FoodSphereClaimType.RestaurantClaimType);
        var branchId = principal.FindFirstValue(FoodSphereClaimType.BranchClaimType);

        var logger = sp.GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(JwtAuthentication));

        if (userId is null || restaurantId is null || branchId is null)
        {
            context.Fail("invalid claims");
            return;
        }

        var workerService = sp.GetRequiredService<WorkerServiceBase>();

        var worker = await workerService.GetWorker(
            e => e, new(
                Guid.Parse(restaurantId),
                short.Parse(branchId),
                short.Parse(userId)));

        if (worker is null)
        {
            context.Fail("worker not found");
            return;
        }

        context.HttpContext.Items[nameof(WorkerUserKey)] = (WorkerUserKey)worker;
        context.HttpContext.Items[nameof(UserType)] = UserType.Worker;
        logger.LogInformation("worker's token passed");
    }

    static async Task OnTokenValidated(TokenValidatedContext context)
    {
        var principal = context.Principal!;
        var sp = context.HttpContext.RequestServices;

        var userTypeClaim = principal.FindFirstValue(FoodSphereClaimType.UserTypeClaimType);

        var logger = sp.GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(JwtAuthentication));

        if (!Enum.TryParse(userTypeClaim, out UserType userType))
        {
            context.Fail("invalid user_type");
            return;
        }

        var envDomainMaster = sp.GetRequiredService<IOptions<EnvDomainMaster>>().Value;
        var envDomainPos = sp.GetRequiredService<IOptions<EnvDomainPos>>().Value;

        var usedSigning = (SymmetricSecurityKey)context.SecurityToken.SigningKey;
        var masterKey = envDomainMaster.GetSecurityKey().Key;
        var posKey = envDomainPos.GetSecurityKey().Key;

        if (userType is UserType.Master && usedSigning.Key.SequenceEqual(masterKey))
        {
            await ValidateMaster(context);
        }
        else if (userType is UserType.Worker && usedSigning.Key.SequenceEqual(posKey))
        {
            await ValidateWorker(context);
        }
        else
        {
            context.Fail("unknown signing key for the user_type");
            return;
        }
    }

    static async Task OnAuthenticationFailed(AuthenticationFailedContext context)
    {
        var sp = context.HttpContext.RequestServices;

        var logger = sp.GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(JwtAuthentication));

        logger.LogWarning("master/worker's token failed: {msg}", context.Exception.Message);
    }
}