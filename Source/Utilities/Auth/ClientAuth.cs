using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using FoodSphere.Data.Models;
using FoodSphere.Services;

namespace FoodSphere.Configurations
{
    public static class JwtClientConfiguration
    {
        public const string SchemeName = nameof(JwtClientConfiguration);

        public static AuthenticationBuilder AddJwtClient(
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
                    ValidAudiences = [
                        credentialService["Domain:master:url"],
                        credentialService["Domain:pos:url"]
                    ],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = [
                        credentialService.GetSecurityKey("Domain:master:signing_key"),
                        credentialService.GetSecurityKey("Domain:pos:signing_key")
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
            var userId = principal.FindFirstValue(AppClaimType.Identity.UserIdClaimType);

            if (userId is null)
            {
                context.Fail("invalid claims");
                return;
            }

            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<MasterUser>>();
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger("JwtClientConfiguration");

            // TokenValidationParameters only check signature, expiration
            var user = await userManager.FindByIdAsync(userId);

            if (user is null)
            {
                context.Fail("master not found");
                return;
            }

            var securityStamp = principal.FindFirstValue(AppClaimType.Identity.SecurityStampClaimType);

            // GetSecurityStampAsync ensure fresh value; user.SecurityStamp possibly outdated
            if (securityStamp is null || securityStamp != await userManager.GetSecurityStampAsync(user))
            {
                context.Fail("security stamp validation failed");
                return;
            }

            context.HttpContext.Items[nameof(MasterUser)] = user;
            logger.LogInformation("master's token passed");
        }

        static async Task ValidateStaff(TokenValidatedContext context)
        {
            var principal = context.Principal!;

            var userId = principal.FindFirstValue(AppClaimType.Identity.UserIdClaimType);
            var restaurantId = principal.FindFirstValue(AppClaimType.RestaurantClaimType);
            var branchId = principal.FindFirstValue(AppClaimType.BranchClaimType);

            if (userId is null || restaurantId is null || branchId is null)
            {
                context.Fail("invalid claims");
                return;
            }

            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger("JwtClientConfiguration");
            var branchService = context.HttpContext.RequestServices.GetRequiredService<BranchService>();

            var staff = await branchService.GetStaff(
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
            logger.LogInformation("staff's token passed");
        }

        public static async Task OnTokenValidated(TokenValidatedContext context)
        {
            var principal = context.Principal!;
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger("JwtClientConfiguration");

            var userTypeClaim = principal.FindFirstValue(AppClaimType.UserTypeClaimType);

            if (!Enum.TryParse(userTypeClaim, out UserType userType))
            {
                context.Fail("invalid user_type");
                return;
            }

            switch (userType)
            {
                case UserType.Master:
                    await ValidateMaster(context);
                    break;

                case UserType.Staff:
                    await ValidateStaff(context);
                    break;
            }

            context.HttpContext.Items[nameof(UserType)] = userType;
        }

        public static async Task OnAuthenticationFailed(AuthenticationFailedContext context)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger("JwtClientConfiguration");

            logger.LogWarning("master/staff's token failed: {msg}", context.Exception.Message);
        }
    }
}

namespace FoodSphere.Services
{
    public class MasterAuthService(
        UserManager<MasterUser> userManager,
        ICredentialService credentialService
    ) {
        readonly UserManager<MasterUser> _userManager = userManager;
        readonly ICredentialService _credentialService = credentialService;

        async Task<ClaimsIdentity> GetSubject(MasterUser user)
        {
            List<Claim> claims = [
                new(AppClaimType.Identity.UserIdClaimType, user.Id),
                new(AppClaimType.Identity.SecurityStampClaimType, user.SecurityStamp ?? string.Empty),
            ];

            var roles = await _userManager.GetRolesAsync(user);

            claims.AddRange(
                roles.Select(role => new Claim(AppClaimType.Identity.RoleClaimType, role))
            );

            return new ClaimsIdentity(claims);
        }

        async Task<Dictionary<string, object>> GetClaims(MasterUser user)
        {
            var claims = new Dictionary<string, object>
            {
                [AppClaimType.UserTypeClaimType] =  UserType.Master.ToString(),
            };

            return claims;
        }

        async Task<SecurityTokenDescriptor> GetTokenDescriptor(MasterUser user)
        {
            return new SecurityTokenDescriptor
            {
                Issuer = _credentialService["Domain:api:url"],
                Audience = _credentialService["Domain:master:url"],
                Subject = await GetSubject(user),
                Claims = await GetClaims(user),
                Expires = DateTime.UtcNow.AddMinutes(300),
                SigningCredentials = _credentialService.SigningCredentials("Domain:master:signing_key"),
            };
        }

        public async Task<string> GenerateToken(MasterUser user)
        {
            var handler = new JsonWebTokenHandler();
            var token = handler.CreateToken(await GetTokenDescriptor(user));

            return token;
        }

        public async Task<bool> IsTwoFactorEnabledAsync(MasterUser user)
        {
            return
                await _userManager.GetTwoFactorEnabledAsync(user) &&
                (await _userManager.GetValidTwoFactorProvidersAsync(user)).Count > 0;
        }
    }

    public class StaffAuthService(
        ICredentialService credentialService
    ) {
        readonly ICredentialService _credentialService = credentialService;

        async Task<ClaimsIdentity> GetSubject(StaffUser user)
        {
            List<Claim> claims = [
                new(AppClaimType.Identity.UserIdClaimType, user.Id.ToString()),
            ];

            return new ClaimsIdentity(claims);
        }

        async Task<Dictionary<string, object>> GetClaims(StaffUser user)
        {
            var claims = new Dictionary<string, object>
            {
                [AppClaimType.RestaurantClaimType] = user.RestaurantId,
                [AppClaimType.BranchClaimType] = user.BranchId,
                [AppClaimType.UserTypeClaimType] = UserType.Staff.ToString(),
            };

            return claims;
        }

        async Task<SecurityTokenDescriptor> GetTokenDescriptor(StaffUser user)
        {
            return new SecurityTokenDescriptor
            {
                Issuer = _credentialService["Domain:api:url"],
                Audience = _credentialService["Domain:pos:url"],
                Subject = await GetSubject(user),
                Claims = await GetClaims(user),
                Expires = DateTime.UtcNow.AddMinutes(300),
                SigningCredentials = _credentialService.SigningCredentials("Domain:pos:signing_key"),
            };
        }

        public async Task<string> GenerateToken(StaffUser user)
        {
            var handler = new JsonWebTokenHandler();
            var token = handler.CreateToken(await GetTokenDescriptor(user));

            return token;
        }
    }
}