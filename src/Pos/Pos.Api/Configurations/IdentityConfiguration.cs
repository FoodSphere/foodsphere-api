using Microsoft.AspNetCore.Identity;

namespace FoodSphere.Pos.Api.Configurations;

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

            options.ClaimsIdentity = FoodSphereClaimType.Identity;
        };
    }
}