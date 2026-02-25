using Microsoft.AspNetCore.Identity;

namespace FoodSphere.Pos.Api.Utility;

public class HealthCheck
{
    public static void Check(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var sp = scope.ServiceProvider;
        var logger = sp.GetRequiredService<ILogger<HealthCheck>>();

        EnsureUserSupport(logger, sp);

        logger.LogInformation("Health check passed");
    }

    static void EnsureUserSupport(ILogger logger, IServiceProvider sp)
    {
        var userManager = sp.GetRequiredService<UserManager<MasterUser>>();

        if (!userManager.SupportsUserEmail)
        {
            throw new NotSupportedException(
                "userManager requires a user store with email support."
            );
        }

        if (!userManager.SupportsUserSecurityStamp)
        {
            throw new NotSupportedException(
                "userManager requires a user store with security stamp support."
            );
        }

        if (!userManager.SupportsUserTwoFactor)
        {
            throw new NotSupportedException(
                "userManager requires a user store with two factor authentication support."
            );
        }
    }
}