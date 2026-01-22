using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodSphere.Data.Models
{
    [PrimaryKey(nameof(RestaurantId), nameof(BranchId), nameof(Id))]
    public class Table : BaseModel<short>
    {
        public Guid RestaurantId { get; set; }
        public short BranchId { get; set; }
        public virtual Branch Branch { get; set; } = null!;

        public string? Name { get; set; }
        public TableStatus Status { get; set; }
    }

    namespace Configurations
    {
        public class TableConfiguration : IEntityTypeConfiguration<Table>
        {
            public void Configure(EntityTypeBuilder<Table> builder)
            {
                builder.HasOne(model => model.Branch)
                    .WithMany(branch => branch.Tables)
                    .HasForeignKey(model => new { model.RestaurantId, model.BranchId })
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}