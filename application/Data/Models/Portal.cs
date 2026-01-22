using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodSphere.Data.Models
{
    public class SelfOrderingPortal : BasePortal
    {
        public Guid BillId { get; set; }
        public virtual Bill Bill { get; set; } = null!;
    }

    public class StaffPortal : BasePortal
    {
        public Guid RestaurantId { get; set; }
        public short BranchId { get; set; }
        public short StaffId { get; set; }
        public virtual StaffUser StaffUser { get; set; } = null!;
    }

    namespace Configurations
    {
        public class SelfOrderingPortalConfiguration : IEntityTypeConfiguration<SelfOrderingPortal>
        {
            public void Configure(EntityTypeBuilder<SelfOrderingPortal> builder)
            {
                builder.HasOne(model => model.Bill)
                    .WithMany(bill => bill.Portals)
                    .HasForeignKey(model => model.BillId)
                    .OnDelete(DeleteBehavior.Cascade);
            }
        }

        public class StaffPortalConfiguration : IEntityTypeConfiguration<StaffPortal>
        {
            public void Configure(EntityTypeBuilder<StaffPortal> builder)
            {
                builder.HasOne(model => model.StaffUser)
                    .WithMany()
                    .HasForeignKey(model => new { model.RestaurantId, model.BranchId, model.StaffId })
                    .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}