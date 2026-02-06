namespace FoodSphere.Infrastructure.Persistence.Configuration;

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

public class RestaurantManagerConfiguration : IEntityTypeConfiguration<RestaurantManager>
{
    public void Configure(EntityTypeBuilder<RestaurantManager> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.MasterId });

        builder.HasOne(e => e.Restaurant)
            .WithMany(e => e.Managers)
            .HasForeignKey(e => new { e.RestaurantId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Master)
            .WithMany(e => e.ManagedRestaurants)
            .HasForeignKey(e => e.MasterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class RestaurantManagerRoleConfiguration : IEntityTypeConfiguration<RestaurantManagerRole>
{
    public void Configure(EntityTypeBuilder<RestaurantManagerRole> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.ManagerId, e.RoleId });

        builder.HasOne(e => e.Manager)
            .WithMany(e => e.Roles)
            .HasForeignKey(e => new { e.RestaurantId, e.ManagerId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Role)
            .WithMany()
            .HasForeignKey(e => new { e.RestaurantId, e.RoleId })
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