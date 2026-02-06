namespace FoodSphere.Infrastructure.Persistence.Configuration;

public class SelfQueuingConfiguration : IEntityTypeConfiguration<Queuing>
{
    public void Configure(EntityTypeBuilder<Queuing> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.BranchId, e.Id });

        builder.HasOne(e => e.Branch)
            .WithMany(e => e.Queuings)
            .HasForeignKey(e => new { e.RestaurantId, e.BranchId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}