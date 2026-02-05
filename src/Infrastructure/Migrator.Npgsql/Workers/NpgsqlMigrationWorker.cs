using System.Diagnostics;

namespace FoodSphere.Worker.Migration;

// https://aspire.dev/integrations/databases/efcore/migrations
public class NpgsqlMigrationWorker(
    ILogger<NpgsqlMigrationWorker> logger,
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime
) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    static readonly ActivitySource activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var activity = activitySource.StartActivity(
            "Migrating database",
            ActivityKind.Client
        );

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FoodSphereDbContext>();
            var strategy = dbContext.Database.CreateExecutionStrategy(); // retry mechanism

            logger.LogInformation("Starting database migrations...");
            await strategy.ExecuteAsync(dbContext.Database.MigrateAsync, ct);

            logger.LogInformation("Migrations applied successfully.");

            // logger.LogInformation("Starting database seeding...");
            // await strategy.ExecuteAsync(async () => await SeedDataAsync(dbContext, ct));

            // logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error applying migrations.");
            activity?.AddException(ex);

            Environment.ExitCode = 1;
        }

        hostApplicationLifetime.StopApplication();
    }

    static async Task SeedDataAsync(FoodSphereDbContext dbContext, CancellationToken ct)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);

        var p1 = new Permission()
        {
            Name = "Menu.Read",
        };

        await dbContext.Set<Permission>().AddAsync(p1, ct);
        await dbContext.SaveChangesAsync(ct);

        await transaction.CommitAsync(ct);
    }
}