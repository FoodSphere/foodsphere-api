using FoodSphere.Consumer.Api.Authentication;
using FoodSphere.Consumer.Api.Configurations;

namespace FoodSphere.Consumer.Api.Authorizations
{
    public class ConsumerAuthorizeAttribute : AuthorizeAttribute
    {
        public ConsumerAuthorizeAttribute()
        {
            Policy = AuthorizationConfiguration.ConsumerPolicy;
        }
    }
}

namespace FoodSphere.Consumer.Api.Configurations
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