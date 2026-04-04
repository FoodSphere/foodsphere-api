namespace FoodSphere.Infrastructure.Persistence.Configuration;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.Id });

        builder.HasOne(e => e.Restaurant)
            .WithMany(e => e.Branches)
            .HasForeignKey(e => e.RestaurantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(e => e.Contact);
    }
}

public class BranchStaffConfiguration : IEntityTypeConfiguration<BranchStaff>
{
    public void Configure(EntityTypeBuilder<BranchStaff> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.BranchId, e.MasterId });

        builder.HasOne(e => e.Branch)
            .WithMany(e => e.BranchStaffs)
            .HasForeignKey(e => new { e.RestaurantId, e.BranchId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Master)
            .WithMany(e => e.ManagedBranches)
            .HasForeignKey(e => e.MasterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class BranchStaffRoleConfiguration : IEntityTypeConfiguration<BranchStaffRole>
{
    public void Configure(EntityTypeBuilder<BranchStaffRole> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.BranchId, e.MasterId, e.RoleId });

        builder.HasOne(e => e.Staff)
            .WithMany(e => e.Roles)
            .HasForeignKey(e => new { e.RestaurantId, e.BranchId, e.MasterId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Role)
            .WithMany()
            .HasForeignKey(e => new { e.RestaurantId, e.RoleId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}