using Microsoft.EntityFrameworkCore;

namespace FoodSphere.Pos.Api.Utilities;

public static class DataSeeder
{
    public static Action<DbContext, bool> Seed(IServiceProvider sp)
    {
        return (context, isChange) =>
        {
            var logger = sp.GetRequiredService<ILoggerFactory>()
                .CreateLogger(nameof(DataSeeder));

            if (isChange)
            {
                logger.LogInformation("UseSeeding, change");
            }
            else
            {
                // why it's always go here?
                logger.LogInformation("UseSeeding, not change");
            }

            // var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();

            // foreach (var roleName in RoleType.GetAll())
            // {
            //     if (!roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
            //     {
            //         roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
            //     }
            // }
        };
    }

    public static Func<DbContext, bool, CancellationToken, Task> SeedAsync(IServiceProvider sp)
    {
        return async (context, isChange, cancellationToken) =>
        {
            var logger = sp.GetRequiredService<ILoggerFactory>()
                .CreateLogger(nameof(DataSeeder));

            if (isChange)
            {
                logger.LogInformation("UseAsyncSeeding, change");
            }
            else
            {
                logger.LogInformation("UseAsyncSeeding, not change");
            }

            // var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();

            // foreach (var roleName in RoleType.GetAll())
            // {
            //     if (!await roleManager.RoleExistsAsync(roleName))
            //     {
            //         await roleManager.CreateAsync(new IdentityRole(roleName));
            //     }
            // }
        };
    }
}