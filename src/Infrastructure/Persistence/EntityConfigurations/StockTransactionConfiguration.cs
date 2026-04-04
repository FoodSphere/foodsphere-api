namespace FoodSphere.Infrastructure.Persistence.Configuration;

public class StockTransactionConfiguration : IEntityTypeConfiguration<StockTransaction>
{
    public void Configure(EntityTypeBuilder<StockTransaction> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.BranchId, e.Id });

        builder.Property(e => e.Amount)
            .HasColumnType("numeric(10,4)");

        builder.Property(e => e.BalanceAfter)
            .HasColumnType("numeric(10,4)");

        // builder.HasIndex(t => new { t.RestaurantId, t.BranchId, t.IngredientId, t.Id })
        //     .IsDescending(false, false, false, true);

        builder.HasOne<Branch>()
            .WithMany(e => e.IngredientStocks)
            .HasForeignKey(e => new { e.RestaurantId, e.BranchId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Ingredient)
            .WithMany()
            .HasForeignKey(e => new { e.RestaurantId, e.IngredientId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.OrderItem)
            .WithMany()
            .HasForeignKey(e => new { e.BillId, e.OrderId, e.OrderItemId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}