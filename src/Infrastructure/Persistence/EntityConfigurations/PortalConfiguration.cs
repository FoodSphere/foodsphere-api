namespace FoodSphere.Infrastructure.Persistence.Configurations;

public class SelfOrderingPortalConfiguration : IEntityTypeConfiguration<SelfOrderingPortal>
{
    public void Configure(EntityTypeBuilder<SelfOrderingPortal> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.Bill)
            .WithMany(e => e.Portals)
            .HasForeignKey(e => e.BillId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class StaffPortalConfiguration : IEntityTypeConfiguration<StaffPortal>
{
    public void Configure(EntityTypeBuilder<StaffPortal> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.StaffUser)
            .WithMany()
            .HasForeignKey(e => new { e.RestaurantId, e.BranchId, e.StaffId })
            .OnDelete(DeleteBehavior.Cascade);
    }
}