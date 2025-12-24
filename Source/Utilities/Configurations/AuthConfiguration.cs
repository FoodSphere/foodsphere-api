using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using FoodSphere.Services;

namespace FoodSphere
{
    public enum UserType
    {
        Admin,
        Master,
        Consumer,
        Staff,
    }

    public static class AppClaimType
    {
        public const string UserTypeClaimType = "user_type";
        public const string BillClaimType = "bill_id";
        public const string BillMemberClaimType = "bill_member_id";
        public const string RestaurantClaimType = "restaurant_id";
        public const string BranchClaimType = "branch_id";

        public static readonly ClaimsIdentityOptions Identity = new()
        {
            RoleClaimType = "role",
            UserNameClaimType = "username",
            UserIdClaimType = "sub",
            EmailClaimType = "email",
            SecurityStampClaimType = "stamp"
        };

        public static IEnumerable<string> GetAll() =>
            typeof(AppClaimType)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.IsLiteral && f.FieldType == typeof(string))
                .Select(f => f.GetValue(null))
                .Cast<string>();
    }
}

namespace FoodSphere.Configurations
{
    public static class AuthenticationConfiguration
    {
        public const string DefaultSchemeName = "FoodSpherePolicy";

        public static AuthenticationBuilder AddFoodSpherePolicy(
            this AuthenticationBuilder builder,
            ICredentialService credentialService
        ) {
            return builder;
            // return builder.AddPolicyScheme(DefaultSchemeName, DefaultSchemeName, PolicyConfigure());
        }

        public static Action<AuthenticationOptions> Configure()
        {
            return options => {
                // options.DefaultScheme = DefaultSchemeName;
            };
        }

        public static Action<PolicySchemeOptions> PolicyConfigure()
        {
            return options => {
                options.ForwardDefaultSelector = ctx =>
                {
                    var path = ctx.Request.Path.Value;

                    if (path is null) {
                        Console.WriteLine("Request path is null.");
                        return JwtClientConfiguration.SchemeName;
                    }

                    // path from Controllers.Consumer.SelfOrderingController
                    if (path.StartsWith("/consumer/ordering")) {
                        Console.WriteLine("Using ordering scheme.");
                        return JwtOrderingConfiguration.SchemeName;
                    }

                    return JwtClientConfiguration.SchemeName;
                };
            };
        }
    }

    public static class GoogleConfiguration
    {
        public static Action<GoogleOptions> Configure(ICredentialService credentialService)
        {
            return options => {
                options.ClientId = credentialService["Google:client_id"]!;
                options.ClientSecret = credentialService["Google:client_secret"]!;
            };
        }
    }

    public static class IdentityConfiguration
    {
        public static Action<IdentityOptions> Configure()
        {
            return options => {
                options.Password.RequiredLength = 4;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;

                options.ClaimsIdentity = AppClaimType.Identity;
            };
        }
    }
}