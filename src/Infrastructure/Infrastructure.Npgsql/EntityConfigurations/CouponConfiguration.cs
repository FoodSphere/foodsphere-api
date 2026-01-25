namespace FoodSphere.Infrastructure.Npgsql.Configurations;

public class DiscountConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.Code });

        builder.Property(e => e.PercentageDiscount)
            .HasColumnType("numeric(7,4)");
    }
}