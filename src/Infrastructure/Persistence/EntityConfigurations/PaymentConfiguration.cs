namespace FoodSphere.Infrastructure.Persistence.Configuration;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(e => new { e.BillId, e.Id });

        builder.HasDiscriminator(e => e.PaymentMethod)
            .HasValue<CashPayment>(CashPayment.CODE)
            .HasValue<StripePayment>(StripePayment.CODE);

        builder.HasOne(e => e.Bill)
            .WithMany(e => e.Payments)
            .HasForeignKey(e => e.BillId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}