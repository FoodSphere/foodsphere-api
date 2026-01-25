namespace FoodSphere.Infrastructure.Npgsql.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.Id });
    }
}

public class ManagerConfiguration : IEntityTypeConfiguration<Manager>
{
    public void Configure(EntityTypeBuilder<Manager> builder)
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

public class ManagerRoleConfiguration : IEntityTypeConfiguration<ManagerRole>
{
    public void Configure(EntityTypeBuilder<ManagerRole> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.BranchId, e.ManagerId, e.RoleId });

        builder.HasOne(e => e.ManagerBranch)
            .WithMany(e => e.Roles)
            .HasForeignKey(e => new { e.RestaurantId, e.BranchId, e.ManagerId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Role)
            .WithMany()
            .HasForeignKey(e => new { e.RestaurantId, e.RoleId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}