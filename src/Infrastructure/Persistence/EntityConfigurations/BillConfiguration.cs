namespace FoodSphere.Infrastructure.Persistence.Configuration;

public class BillConfiguration : IEntityTypeConfiguration<Bill>
{
    public void Configure(EntityTypeBuilder<Bill> builder)
    {
        builder.HasKey(e => new { e.Id });

        builder.HasOne(e => e.Table)
            .WithMany()
            .HasForeignKey(e => new { e.RestaurantId, e.BranchId, e.TableId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<StaffUser>()
            .WithMany()
            .HasForeignKey(e => new { e.RestaurantId, e.BranchId, e.IssuerId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class BillMemberConfiguration : IEntityTypeConfiguration<BillMember>
{
    public void Configure(EntityTypeBuilder<BillMember> builder)
    {
        builder.HasKey(e => new { e.BillId, e.Id });
    }
}

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(e => new { e.BillId, e.Id });

        builder.HasOne<StaffUser>()
            .WithMany()
            .HasForeignKey(e => new { e.RestaurantId, e.BranchId, e.IssuerId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.BillMember)
            .WithMany(e => e.Orders)
            .HasForeignKey(e => new { e.BillId, e.BillMemberId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(e => new { e.BillId, e.RestaurantId, e.OrderId, e.MenuId });

        builder.HasOne(e => e.Order)
            .WithMany(e => e.Items)
            .HasForeignKey(e => new { e.BillId, e.OrderId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Menu)
            .WithMany()
            .HasForeignKey(e => new { e.RestaurantId, e.MenuId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}