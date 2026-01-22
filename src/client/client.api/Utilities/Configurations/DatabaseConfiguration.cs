using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using FoodSphere.Data.Models;
using FoodSphere.Data.Models.Configurations;

namespace FoodSphere;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<MasterUser>(options)
{
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        HandleTrackTime();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        HandleTrackTime();

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        // modelBuilder.PermissionSeeding();
    }

    void HandleTrackTime()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is ITrackableModel entity)
            {
                var now = DateTime.UtcNow;

                switch (entry.State)
                {
                    case EntityState.Added:
                        entity.UpdateTime = now;
                        break;

                    case EntityState.Modified:
                        entity.UpdateTime = now;
                        break;
                }
            }
        }
    }
}

public static class Seeding
{
    public static Action<DbContext, bool> Seed(IServiceProvider serviceProvider)
    {
        return (context, isChange) =>
        {
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>()
                .CreateLogger("Seeding");

            if (isChange)
            {
                logger.LogInformation("UseSeeding, change");
            }
            else
            {
                // why it's always go here?
                logger.LogInformation("UseSeeding, not change");
            }

            // var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // foreach (var roleName in RoleType.GetAll())
            // {
            //     if (!roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
            //     {
            //         roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
            //     }
            // }
        };
    }

    public static Func<DbContext, bool, CancellationToken, Task> SeedAsync(IServiceProvider serviceProvider)
    {
        return async (context, isChange, cancellationToken) =>
        {
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>()
                .CreateLogger("Seeding");

            if (isChange)
            {
                logger.LogInformation("UseAsyncSeeding, change");
            }
            else
            {
                logger.LogInformation("UseAsyncSeeding, not change");
            }

            // var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

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