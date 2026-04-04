namespace FoodSphere.Infrastructure.Persistence.Configuration;

public class MasterConfiguration : IEntityTypeConfiguration<MasterUser>
{
    public void Configure(EntityTypeBuilder<MasterUser> builder)
    {
    }
}

public class WorkerConfiguration : IEntityTypeConfiguration<WorkerUser>
{
    public void Configure(EntityTypeBuilder<WorkerUser> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.BranchId, e.Id });

        builder.HasOne(e => e.Branch)
            .WithMany(e => e.Workers)
            .HasForeignKey(e => new { e.RestaurantId, e.BranchId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class WorkerRoleConfiguration : IEntityTypeConfiguration<WorkerRole>
{
    public void Configure(EntityTypeBuilder<WorkerRole> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.BranchId, e.WorkerId, e.RoleId });

        builder.HasOne(e => e.Worker)
            .WithMany(e => e.Roles)
            .HasForeignKey(e => new { e.RestaurantId, e.BranchId, e.WorkerId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Role)
            .WithMany()
            .HasForeignKey(e => new { e.RestaurantId, e.RoleId })
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ConsumerConfiguration : IEntityTypeConfiguration<ConsumerUser>
{
    public void Configure(EntityTypeBuilder<ConsumerUser> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.Email)
            .IsUnique();

        builder.HasIndex(e => e.UserName)
            .IsUnique();
    }
}