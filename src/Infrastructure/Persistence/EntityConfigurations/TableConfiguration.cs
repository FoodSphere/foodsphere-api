namespace FoodSphere.Infrastructure.Persistence.Configurations;

public class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.BranchId, e.Id });

        builder.HasOne(e => e.Branch)
            .WithMany(e => e.Tables)
            .HasForeignKey(e => new { e.RestaurantId, e.BranchId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}