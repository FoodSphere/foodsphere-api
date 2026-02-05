namespace FoodSphere.Infrastructure.Persistence.Configurations;

public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.BranchId, e.IngredientId });

        builder.Property(e => e.Amount)
            .HasColumnType("numeric(10,4)");

        builder.HasOne(e => e.Branch)
            .WithMany(e => e.IngredientStocks)
            .HasForeignKey(e => new { e.RestaurantId, e.BranchId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Ingredient)
            .WithMany()
            .HasForeignKey(e => new { e.RestaurantId, e.IngredientId })
            .OnDelete(DeleteBehavior.Cascade);
    }
}