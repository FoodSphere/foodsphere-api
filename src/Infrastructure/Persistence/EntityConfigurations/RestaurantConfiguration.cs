namespace FoodSphere.Infrastructure.Persistence.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.HasKey(e => e.Id);
    }
}

public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.Owner)
            .WithMany(e => e.OwnedRestaurants)
            .HasForeignKey(e => e.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.Name });

        builder.HasOne<Restaurant>()
            .WithMany(e => e.Tags)
            .HasForeignKey(e => e.RestaurantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}