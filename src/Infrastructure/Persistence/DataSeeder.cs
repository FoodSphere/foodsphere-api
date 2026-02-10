using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FoodSphere.Infrastructure.Persistence;

public static class DbContextOptionsBuilderExtension
{
    extension(DbContextOptionsBuilder optionsBuilder)
    {
        public DbContextOptionsBuilder UseFoodSphereSeeding(IServiceProvider sp)
        {
            return optionsBuilder
                .UseSeeding(DataSeeder.Seed(sp))
                .UseAsyncSeeding(DataSeeder.SeedAsync(sp)); // depend on .ensureAsync()?
        }
    }
}

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
        };
    }

    public static Func<DbContext, bool, CancellationToken, Task> SeedAsync(IServiceProvider sp)
    {
        return async (context, isChange, ct) =>
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
        };
    }
}