using FoodSphere.Consumer.Api.Authentication;
using FoodSphere.Consumer.Api.Configuration;

namespace FoodSphere.Consumer.Api.Authorization
{
    public class ConsumerAuthorizeAttribute : AuthorizeAttribute
    {
        public ConsumerAuthorizeAttribute()
        {
            Policy = AuthorizationConfiguration.ConsumerPolicy;
        }
    }
}

namespace FoodSphere.Consumer.Api.Configuration
{
    public static class AuthorizationConfiguration
    {
        public const string ConsumerPolicy = nameof(ConsumerPolicy);

        public static Action<AuthorizationOptions> Configure()
        {
            return options => {
                options.AddPolicy(ConsumerPolicy, policy =>
                {
                    policy.AddAuthenticationSchemes(JwtAuthentication.SchemeName);
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(FoodSphereClaimType.Identity.UserIdClaimType);
                });
            };
        }
    }
}