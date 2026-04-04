using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FoodSphere.Infrastructure.Persistence;

public class FoodSphereDbContext(DbContextOptions<FoodSphereDbContext> options) : IdentityDbContext<MasterUser>(options)
{
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        HandleTimestamp();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess, CancellationToken ct = default)
    {
        HandleTimestamp();

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, ct);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FoodSphereDbContext).Assembly);
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();

        modelBuilder.Entity<Permission>().HasData(PERMISSION.GetAll());
    }

    void HandleTimestamp()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State is EntityState.Added)
            {
                if (entry.Entity is IEntityModel entity)
                {
                    entity.CreateTime = DateTime.UtcNow;
                }
            }
            if (entry.State is EntityState.Modified)
            {
                if (entry.Entity is IUpdatableEntityModel entity)
                {
                    entity.UpdateTime = DateTime.UtcNow;
                }
            }
            else if (entry.State is EntityState.Deleted)
            {
                if (entry.Entity is ISoftDeleteEntityModel entity)
                {
                    entry.State = EntityState.Modified;
                    entity.DeleteTime = DateTime.UtcNow;
                }
            }
        }
    }
}