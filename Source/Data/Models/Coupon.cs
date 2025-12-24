using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodSphere.Data.Models
{
    [PrimaryKey(nameof(RestaurantId), nameof(Code))]
    public class Coupon : TrackableModel
    {
        public Guid RestaurantId { get; set; }
        public required string Code { get; set; }

        public decimal PercentageDiscount { get; set; }
        public int FixedDiscount { get; set; }
        public int MaxUsage { get; set; }

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
    }

    namespace Configurations
    {
        public class DiscountConfiguration : IEntityTypeConfiguration<Coupon>
        {
            public void Configure(EntityTypeBuilder<Coupon> builder)
            {
            }
        }
    }
}