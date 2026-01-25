namespace FoodSphere.Infrastructure.Npgsql.Configurations;

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.Id });
    }
}

public class MenuComponentConfiguration : IEntityTypeConfiguration<MenuComponent>
{
    public void Configure(EntityTypeBuilder<MenuComponent> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.ParentMenuId, e.ChildMenuId });

        builder.HasOne(e => e.ParentMenu)
            .WithMany(e => e.Components)
            .HasForeignKey(e => new { e.RestaurantId, e.ParentMenuId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ChildMenu)
            .WithMany()
            .HasForeignKey(e => new { e.RestaurantId, e.ChildMenuId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class MenuTagConfiguration : IEntityTypeConfiguration<MenuTag>
{
    public void Configure(EntityTypeBuilder<MenuTag> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.MenuId, e.TagId });

        builder.HasOne(e => e.Menu)
            .WithMany(e => e.MenuTags)
            .HasForeignKey(e => new { e.RestaurantId, e.MenuId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Tag)
            .WithMany(e => e.MenuTags)
            .HasForeignKey(e => new { e.RestaurantId, e.TagId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}