using System.Security.Claims;
using MessagePack;
using FoodSphere.Resource.Api.Authentication;

// at Program.cs
// builder.Services.AddSingleton<IAuthorizationPolicyProvider, ClientPolicyProvider>();
// builder.Services.AddSingleton<IAuthorizationHandler, ClientHandler>();

namespace FoodSphere.Resource.Api.Authorization;

class PolicyPrefix
{
    public const string Resource = "RESOURCE;";
}

public enum OperationType
{
    OrderRead,
    OrderWrite,
    MenuRead,
}

public class NewResourceAuthorizeAttribute : AuthorizeAttribute
{
    void SetPolicy(ResourceRequirement requirement)
    {
        var serialized = MessagePackSerializer.Serialize(requirement);
        Policy = PolicyPrefix.Resource + Convert.ToBase64String(serialized);
    }

    public NewResourceAuthorizeAttribute(params OperationType[] permissions)
    {
        var requirement = new ResourceRequirement
        {
            Permissions = permissions,
        };

        SetPolicy(requirement);
    }

    public NewResourceAuthorizeAttribute(UserType userType, params OperationType[] permissions)
    {
        var requirement = new ResourceRequirement
        {
            Permissions = permissions,
            UserType = userType,
        };

        SetPolicy(requirement);
    }
}

// https://learn.microsoft.com/en-us/aspnet/core/security/authorization/iauthorizationpolicyprovider
public class ResourcePolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
{
    readonly DefaultAuthorizationPolicyProvider backupPolicyProvider = new(options);

    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(PolicyPrefix.Resource))
        {
            var serialized = policyName[PolicyPrefix.Resource.Length..];
            var bytes = Convert.FromBase64String(serialized);
            var requirement = MessagePackSerializer.Deserialize<ResourceRequirement>(bytes);
            var policy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtAuthentication.SchemeName)
                .RequireAuthenticatedUser()
                .RequireClaim(FoodSphereClaimType.Identity.UserIdClaimType)
                .RequireClaim(FoodSphereClaimType.UserTypeClaimType)
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

[MessagePackObject]
public class ResourceRequirement : IAuthorizationRequirement
{
    [Key(0)]
    public UserType? UserType { get; set; } = null;
    [Key(1)]
    public OperationType[] Permissions { get; set; } = [];
}

public class ResourceHandler(ILogger<ResourceHandler> logger) : AuthorizationHandler<ResourceRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ResourceRequirement requirement
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
            var userTypeClaim = context.User.FindFirstValue(FoodSphereClaimType.UserTypeClaimType) ?? throw new InvalidOperationException();
            var userType = Enum.Parse<UserType>(userTypeClaim);

            logger.LogInformation("requirement: {RequireUserType}, parsed: {UserType}", requirement.UserType, userType);

            if (requirement.UserType != userType)
            {
                context.Fail();
                return;
            }
        }

        // but we must known before hand that which restaurant/branch need to be checked permissions with?
        context.Succeed(requirement);
    }
}