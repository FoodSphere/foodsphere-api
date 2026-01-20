using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using MessagePack;
using FoodSphere.Data;
using FoodSphere.Utilities;
using FoodSphere.Configurations;
using System.Security.Claims;

namespace FoodSphere.Data {
    public class PolicyPrefix
    {
        public const string Client = "CLIENT;";
    }

    public enum PermissionType
    {
        OrderRead,
        OrderWrite,
        MenuRead,
    }
}

namespace FoodSphere.Controllers
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        public AdminAuthorizeAttribute()
        {
            Policy = AuthorizationConfiguration.AdminPolicy;
        }
    }

    public class ClientAuthorizeAttribute : AuthorizeAttribute
    {
        void SetPolicy(ClientRequirement requirement)
        {
            var serialized = MessagePackSerializer.Serialize(requirement);
            Policy = PolicyPrefix.Client + Convert.ToBase64String(serialized);
        }

        public ClientAuthorizeAttribute(params PermissionType[] permissions)
        {
            var requirement = new ClientRequirement
            {
                Permissions = permissions,
            };

            SetPolicy(requirement);
        }

        public ClientAuthorizeAttribute(UserType userType, params PermissionType[] permissions)
        {
            var requirement = new ClientRequirement
            {
                Permissions = permissions,
                UserType = userType,
            };

            SetPolicy(requirement);
        }
    }

    public class ConsumerAuthorizeAttribute : AuthorizeAttribute
    {
        public ConsumerAuthorizeAttribute()
        {
            Policy = AuthorizationConfiguration.ConsumerPolicy;
        }
    }

    public class OrderingAuthorizeAttribute : AuthorizeAttribute
    {
        public OrderingAuthorizeAttribute()
        {
            Policy = AuthorizationConfiguration.OrderingPolicy;
        }
    }
}

namespace FoodSphere.Configurations
{
    public static class AuthorizationConfiguration
    {
        public const string AdminPolicy = "Admin";
        public const string ConsumerPolicy = "Consumer";
        public const string OrderingPolicy = "SelfOrdering";

        public static Action<AuthorizationOptions> Configure()
        {
            return options => {
                options.AddPolicy(AdminPolicy, policy =>
                {
                    policy.AddAuthenticationSchemes(JwtAdminConfiguration.SchemeName);
                    policy.RequireClaim(AppClaimType.Identity.UserIdClaimType);
                });

                options.AddPolicy(ConsumerPolicy, policy =>
                {
                    policy.AddAuthenticationSchemes(JwtConsumerConfiguration.SchemeName);
                    policy.RequireClaim(AppClaimType.Identity.UserIdClaimType);
                });

                options.AddPolicy(OrderingPolicy, policy =>
                {
                    policy.AddAuthenticationSchemes(JwtOrderingConfiguration.SchemeName);
                    policy.RequireClaim(AppClaimType.BillClaimType);
                    policy.RequireClaim(AppClaimType.BillMemberClaimType);
                });
            };
        }
    }
}

namespace FoodSphere.Services
{
    // https://learn.microsoft.com/en-us/aspnet/core/security/authorization/iauthorizationpolicyprovider
    public class AppPolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
    {
        readonly DefaultAuthorizationPolicyProvider backupPolicyProvider = new(options);

        public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(PolicyPrefix.Client))
            {
                var serialized = policyName[PolicyPrefix.Client.Length..];
                var bytes = Convert.FromBase64String(serialized);
                var requirement = MessagePackSerializer.Deserialize<ClientRequirement>(bytes);

                var policy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtClientConfiguration.SchemeName)
                    .RequireAuthenticatedUser()
                    .RequireClaim(AppClaimType.Identity.UserIdClaimType)
                    .RequireClaim(AppClaimType.UserTypeClaimType)
                    .AddRequirements(requirement)
                    .Build();

                return policy;
            }

            return await backupPolicyProvider.GetPolicyAsync(policyName);
        }

        // for simply `[Authorize]`
        public async Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return await backupPolicyProvider.GetDefaultPolicyAsync();
        }

        // when no metadata (eg. [Authorize], [AllowAnonymous]) attribute is specified
        public async Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return await backupPolicyProvider.GetFallbackPolicyAsync();
        }
    }
}

namespace FoodSphere.Utilities
{
    [MessagePackObject]
    public class ClientRequirement : IAuthorizationRequirement
    {
        [Key(0)]
        public UserType? UserType { get; set; } = null;
        [Key(1)]
        public PermissionType[] Permissions { get; set; } = [];
    }

    public class ClientHandler(ILogger<ClientHandler> logger) : AuthorizationHandler<ClientRequirement>
    {
        readonly ILogger<ClientHandler> _logger = logger;

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ClientRequirement requirement
        ) {
            // authentication middleware failed -> [IsAuthenticated == false]
            // request still proceeds to this (authorization middleware)
            if (context.User.Identity?.IsAuthenticated != true)
            {
                // multiple policy requirements -> all handlers are running
                // .RequireAuthenticatedUser() handle rejection (DenyAnonymousAuthorizationRequirement)
                return;
            }

            if (requirement.UserType is not null)
            {
                // confidently thrown exception here, because
                // UserTypeClaimType already checked in JwtClientConfiguration.OnTokenValidated()
                var userTypeClaim = context.User.FindFirstValue(AppClaimType.UserTypeClaimType) ?? throw new InvalidOperationException();
                var userType = Enum.Parse<UserType>(userTypeClaim);

                _logger.LogInformation("requirement: {RequireUserType}, parsed: {UserType}", requirement.UserType, userType);

                if (requirement.UserType != userType)
                {
                    context.Fail();
                    return;
                }
            }

            // but we must known which restaurant/branch before check permissions?
            context.Succeed(requirement);
        }
    }
}