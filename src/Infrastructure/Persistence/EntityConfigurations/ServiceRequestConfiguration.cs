namespace FoodSphere.Infrastructure.Persistence.Configuration;

public class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
{
    public void Configure(EntityTypeBuilder<ServiceRequest> builder)
    {
        builder.HasKey(e => new { e.BillId, e.Id });

        builder.HasOne(e => e.Bill)
            .WithMany()
            .HasForeignKey(e => e.BillId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}