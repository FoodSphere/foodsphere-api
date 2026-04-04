namespace FoodSphere.Infrastructure.Persistence.Configuration;

public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.Owner)
            .WithMany(e => e.OwnedRestaurants)
            .HasForeignKey(e => e.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(e => e.Contact);
    }
}

public class RestaurantStaffConfiguration : IEntityTypeConfiguration<RestaurantStaff>
{
    public void Configure(EntityTypeBuilder<RestaurantStaff> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.MasterId });

        builder.HasOne(e => e.Restaurant)
            .WithMany(e => e.Staffs)
            .HasForeignKey(e => new { e.RestaurantId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Master)
            .WithMany(e => e.ManagedRestaurants)
            .HasForeignKey(e => e.MasterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class RestaurantStaffRoleConfiguration : IEntityTypeConfiguration<RestaurantStaffRole>
{
    public void Configure(EntityTypeBuilder<RestaurantStaffRole> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.MasterId, e.RoleId });

        builder.HasOne(e => e.Staff)
            .WithMany(e => e.Roles)
            .HasForeignKey(e => new { e.RestaurantId, e.MasterId })
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
        builder.HasKey(e => new { e.RestaurantId, e.Id });

        builder.HasOne<Restaurant>()
            .WithMany(e => e.Tags)
            .HasForeignKey(e => e.RestaurantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.RestaurantId, e.Name })
            .IsUnique();
    }
}