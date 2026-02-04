using FoodSphere.Pos.Api.Authentication;
using FoodSphere.Pos.Api.Configurations;

namespace FoodSphere.Pos.Api.Authorizations
{
    public class PosAuthorizeAttribute : AuthorizeAttribute
    {
        public PosAuthorizeAttribute()
        {
            Policy = AuthorizationConfiguration.PosPolicy;
        }
    }

    public class MasterAuthorizeAttribute : AuthorizeAttribute
    {
        public MasterAuthorizeAttribute()
        {
            Policy = AuthorizationConfiguration.MasterPolicy;
        }
    }
}

namespace FoodSphere.Pos.Api.Configurations
{
    public static class AuthorizationConfiguration
    {
        public const string MasterPolicy = nameof(MasterPolicy);
        public const string PosPolicy = nameof(PosPolicy);

        public static Action<AuthorizationOptions> Configure()
        {
            return options =>
            {
                options.AddPolicy(PosPolicy, policy =>
                 {
                    policy.AddAuthenticationSchemes(JwtAuthentication.SchemeName);
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(FoodSphereClaimType.Identity.UserIdClaimType);
                    policy.RequireClaim(FoodSphereClaimType.UserTypeClaimType, UserType.Master.ToString(), UserType.Staff.ToString());
                 });

                options.AddPolicy(MasterPolicy, policy =>
                {
                    policy.AddAuthenticationSchemes(JwtAuthentication.SchemeName);
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(FoodSphereClaimType.Identity.UserIdClaimType);
                    policy.RequireClaim(FoodSphereClaimType.UserTypeClaimType, UserType.Master.ToString());
                });
            };
        }
    }
}