namespace FoodSphere.Worker.Migration;

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