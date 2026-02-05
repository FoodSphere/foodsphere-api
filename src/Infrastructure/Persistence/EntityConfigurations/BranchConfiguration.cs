namespace FoodSphere.Infrastructure.Persistence.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.Id });
    }
}

public class BranchManagerConfiguration : IEntityTypeConfiguration<BranchManager>
{
    public void Configure(EntityTypeBuilder<BranchManager> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.BranchId, e.MasterId });

        builder.HasOne(e => e.Branch)
            .WithMany(e => e.BranchManagers)
            .HasForeignKey(e => new { e.RestaurantId, e.BranchId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Master)
            .WithMany(e => e.ManagedBranches)
            .HasForeignKey(e => e.MasterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class BranchManagerRoleConfiguration : IEntityTypeConfiguration<BranchManagerRole>
{
    public void Configure(EntityTypeBuilder<BranchManagerRole> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.BranchId, e.ManagerId, e.RoleId });

        builder.HasOne(e => e.Manager)
            .WithMany(e => e.Roles)
            .HasForeignKey(e => new { e.RestaurantId, e.BranchId, e.ManagerId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Role)
            .WithMany()
            .HasForeignKey(e => new { e.RestaurantId, e.RoleId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}