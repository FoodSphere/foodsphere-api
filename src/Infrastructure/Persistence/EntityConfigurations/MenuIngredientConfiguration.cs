namespace FoodSphere.Infrastructure.Persistence.Configurations;

public class MenuIngredientConfiguration : IEntityTypeConfiguration<MenuIngredient>
{
    public void Configure(EntityTypeBuilder<MenuIngredient> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.MenuId, e.IngredientId });

        builder.Property(e => e.Amount)
            .HasColumnType("numeric(10,4)");

        builder.HasOne(e => e.Menu)
            .WithMany(e => e.MenuIngredients)
            .HasForeignKey(e => new { e.RestaurantId, e.MenuId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Ingredient)
            .WithMany(e => e.MenuIngredients)
            .HasForeignKey(e => new { e.RestaurantId, e.IngredientId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
