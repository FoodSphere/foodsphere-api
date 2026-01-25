namespace FoodSphere.Infrastructure.Npgsql.Configurations;

public class MasterConfiguration : IEntityTypeConfiguration<MasterUser>
{
    public void Configure(EntityTypeBuilder<MasterUser> builder)
    {
    }
}

public class StaffConfiguration : IEntityTypeConfiguration<StaffUser>
{
    public void Configure(EntityTypeBuilder<StaffUser> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.BranchId, e.Id });

        builder.HasOne(e => e.Branch)
            .WithMany(e => e.Staffs)
            .HasForeignKey(e => new { e.RestaurantId, e.BranchId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class StaffRoleConfiguration : IEntityTypeConfiguration<StaffRole>
{
    public void Configure(EntityTypeBuilder<StaffRole> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.BranchId, e.StaffId, e.RoleId });

        builder.HasOne(e => e.Staff)
            .WithMany(e => e.Roles)
            .HasForeignKey(e => new { e.RestaurantId, e.BranchId, e.StaffId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Role)
            .WithMany()
            .HasForeignKey(e => new { e.RestaurantId, e.RoleId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ConsumerConfiguration : IEntityTypeConfiguration<ConsumerUser>
{
    public void Configure(EntityTypeBuilder<ConsumerUser> builder)
    {
        builder.HasKey(e => e.Id);
    }
}