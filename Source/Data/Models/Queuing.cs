using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodSphere.Data.Models
{
    [PrimaryKey(nameof(RestaurantId), nameof(BranchId), nameof(Id))]
    public class Queuing : BaseModel<string>
    {
        public Guid RestaurantId { get; set; }
        public short BranchId { get; set; }
        public virtual Branch Branch { get; set; } = null!;

        public Guid? ConsumerId { get; set; }
        public virtual ConsumerUser? Consumer { get; set; } = null!;

        public short? Pax { get; set; }
    }

    namespace Configurations
    {
        public class SelfQueuingConfiguration : IEntityTypeConfiguration<Queuing>
        {
            public void Configure(EntityTypeBuilder<Queuing> builder)
            {
                builder.HasOne(model => model.Branch)
                    .WithMany(branch => branch.Queuings)
                    .HasForeignKey(model => new { model.RestaurantId, model.BranchId })
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}