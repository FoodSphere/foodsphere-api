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
                ValidAudiences = [
                    // envDomainMaster.hostname,
                    envDomainPos.hostname
                ],
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = [
                    envDomainMaster.GetSecurityKey(),
                    envDomainPos.GetSecurityKey()
                ]
            };
            options.Events = new JwtBearerEvents
            {
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

        context.HttpContext.Items[nameof(MasterUser)] = user;
        context.HttpContext.Items[nameof(UserType)] = UserType.Master;
        logger.LogInformation("master's token passed");
    }

    static async Task ValidateStaff(TokenValidatedContext context)
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

        var staffService = sp.GetRequiredService<StaffService>();

        var staff = await staffService.GetStaff(
            Guid.Parse(restaurantId),
            short.Parse(branchId),
            short.Parse(userId)
        );

        if (staff is null)
        {
            context.Fail("staff not found");
            return;
        }

        context.HttpContext.Items[nameof(StaffUser)] = staff;
        context.HttpContext.Items[nameof(UserType)] = UserType.Staff;
        logger.LogInformation("staff's token passed");
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

        var signing = (SymmetricSecurityKey)context.SecurityToken.SigningKey;
        var masterKey = envDomainMaster.GetSecurityKey().Key;
        var posKey = envDomainPos.GetSecurityKey().Key;

        if (userType is UserType.Master && signing.Key.SequenceEqual(masterKey))
        {
            await ValidateMaster(context);
        }
        else if (userType is UserType.Staff && signing.Key.SequenceEqual(posKey))
        {
            await ValidateStaff(context);
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

        logger.LogWarning("master/staff's token failed: {msg}", context.Exception.Message);
    }
}