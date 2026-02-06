namespace FoodSphere.Infrastructure.Persistence.Configuration;

public class DiscountConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.HasKey(e => new { e.RestaurantId, e.Code });

        builder.Property(e => e.PercentageDiscount)
            .HasColumnType("numeric(7,4)");
    }
}