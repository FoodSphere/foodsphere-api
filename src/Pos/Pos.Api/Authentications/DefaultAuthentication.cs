using Microsoft.AspNetCore.Authentication;

namespace FoodSphere.Pos.Api.Authentication;

// authentication scheme that defines a policy to select other authentication schemes
public static class DefaultAuthentication
{
    public const string SchemeName = nameof(DefaultAuthentication);

    extension(AuthenticationBuilder builder)
    {
        public AuthenticationBuilder AddDefaultPolicyScheme(IServiceCollection services)
        {
            using var sp = services.BuildServiceProvider();

            return builder.AddPolicyScheme(SchemeName, SchemeName, PolicySchemeConfigure(sp));
        }
    }

    public static Action<AuthenticationOptions> Configure(IServiceCollection services)
    {
        using var sp = services.BuildServiceProvider();

        // my reminder: Configure() shouldn't capture sp to lambda and use `sp` after it was disposed
        // me, not AI btw.
        return options => {
            // options.DefaultScheme = SchemeName;
        };
    }

    static Action<PolicySchemeOptions> PolicySchemeConfigure(IServiceProvider sp)
    {
        return options => {
            options.ForwardDefaultSelector = context =>
            {
                var path = context.Request.Path.Value;

                if (path is null) {
                    return "";
                }

                return JwtAuthentication.SchemeName;
            };
        };
    }
}