namespace FoodSphere.Infrastructure.Persistence.Configuration;

public class SelfOrderingPortalConfiguration : IEntityTypeConfiguration<OrderingPortal>
{
    public void Configure(EntityTypeBuilder<OrderingPortal> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.Bill)
            .WithMany(e => e.Portals)
            .HasForeignKey(e => e.BillId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class WorkerPortalConfiguration : IEntityTypeConfiguration<WorkerPortal>
{
    public void Configure(EntityTypeBuilder<WorkerPortal> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.WorkerUser)
            .WithMany()
            .HasForeignKey(e => new { e.RestaurantId, e.BranchId, e.WorkerId })
            .OnDelete(DeleteBehavior.Cascade);
    }
}