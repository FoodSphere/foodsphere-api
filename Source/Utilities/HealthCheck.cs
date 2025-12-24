using Microsoft.AspNetCore.Identity;
using FoodSphere.Data.Models;

namespace FoodSphere.Services;

public class HealthCheck
{
    public static void Check(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var logger = serviceProvider.GetRequiredService<ILogger<HealthCheck>>();

        EnsureUserSupport(logger, serviceProvider);

        logger.LogInformation("Health check passed");
    }

    static void EnsureUserSupport(ILogger logger, IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<MasterUser>>();

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