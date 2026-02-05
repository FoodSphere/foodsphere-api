using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FoodSphere.Infrastructure.Persistence;

public class FoodSphereDbContext(DbContextOptions<FoodSphereDbContext> options) : IdentityDbContext<MasterUser>(options)
{
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        HandleTrackTime();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken ct = default)
    {
        HandleTrackTime();

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, ct);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FoodSphereDbContext).Assembly);
        // modelBuilder.PermissionSeeding();
    }

    void HandleTrackTime()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is ITrackableEntity entity)
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