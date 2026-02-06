using FoodSphere.SelfOrdering.Api.Authentication;
using FoodSphere.SelfOrdering.Api.Configuration;

namespace FoodSphere.SelfOrdering.Api.Authorization
{
    public class SelfOrderingAuthorizeAttribute : AuthorizeAttribute
    {
        public SelfOrderingAuthorizeAttribute()
        {
            Policy = AuthorizationConfiguration.OrderingPolicy;
        }
    }
}

namespace FoodSphere.SelfOrdering.Api.Configuration
{
    public static class AuthorizationConfiguration
    {
        public const string OrderingPolicy = nameof(OrderingPolicy);

        public static Action<AuthorizationOptions> Configure()
        {
            return options => {
                options.AddPolicy(OrderingPolicy, policy =>
                {
                    policy.AddAuthenticationSchemes(JwtAuthentication.SchemeName);
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(FoodSphereClaimType.BillClaimType);
                    policy.RequireClaim(FoodSphereClaimType.BillMemberClaimType);
                });
            };
        }
    }
}