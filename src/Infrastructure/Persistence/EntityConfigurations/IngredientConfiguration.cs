namespace FoodSphere.Infrastructure.Persistence.Configuration;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.Id });
    }
}

public class IngredientTagConfiguration : IEntityTypeConfiguration<IngredientTag>
{
    public void Configure(EntityTypeBuilder<IngredientTag> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.IngredientId, e.TagId });

        builder.HasOne(e => e.Ingredient)
            .WithMany(e => e.IngredientTags)
            .HasForeignKey(e => new { e.RestaurantId, e.IngredientId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Tag)
            .WithMany(e => e.IngredientTags)
            .HasForeignKey(e => new { e.RestaurantId, e.TagId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}