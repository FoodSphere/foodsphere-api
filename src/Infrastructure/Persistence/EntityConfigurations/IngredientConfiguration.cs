namespace FoodSphere.Infrastructure.Persistence.Configuration;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.Id });

        builder.HasOne(e => e.Restaurant)
            .WithMany(e => e.Ingredients)
            .HasForeignKey(e => e.RestaurantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class TagIngredientConfiguration : IEntityTypeConfiguration<TagIngredient>
{
    public void Configure(EntityTypeBuilder<TagIngredient> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.IngredientId, e.TagId });

        builder.HasOne(e => e.Ingredient)
            .WithMany(e => e.Tags)
            .HasForeignKey(e => new { e.RestaurantId, e.IngredientId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Tag)
            .WithMany(e => e.TagIngredients)
            .HasForeignKey(e => new { e.RestaurantId, e.TagId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}