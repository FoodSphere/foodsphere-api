using Microsoft.AspNetCore.Authorization;
using FoodSphere.Resource.Api.Authentication;
using FoodSphere.Resource.Api.Configurations;

namespace FoodSphere.Resource.Api.Authorization
{
    public class ResourceAuthorizeAttribute : AuthorizeAttribute
    {
        public ResourceAuthorizeAttribute()
        {
            Policy = AuthorizationConfiguration.ResourcePolicy;
        }
    }
}

namespace FoodSphere.Resource.Api.Configurations
{
    public static class AuthorizationConfiguration
    {
        public const string ResourcePolicy = nameof(ResourcePolicy);

        public static Action<AuthorizationOptions> Configure()
        {
            return options =>
            {
                options.AddPolicy(ResourcePolicy, policy =>
                {
                    policy.AddAuthenticationSchemes(JwtAuthentication.SchemeName);
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(FoodSphereClaimType.Identity.UserIdClaimType);
                });
            };
        }
    }
}