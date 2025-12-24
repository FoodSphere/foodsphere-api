using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodSphere.Data.Models
{
    [PrimaryKey(nameof(RestaurantId), nameof(BranchId), nameof(IngredientId))]
    public class Stock : TrackableModel
    {
        public Guid RestaurantId { get; set; }

        public short BranchId { get; set; }
        public virtual Branch Branch { get; set; } = null!;

        public short IngredientId { get; set; }
        public virtual Ingredient Ingredient { get; set; } = null!;

        [Column(TypeName = "numeric(10,4)")]
        public decimal Amount { get; set; }

        public StockStatus Status { get; set; }
    }

    namespace Configurations
    {
        public class StockConfiguration : IEntityTypeConfiguration<Stock>
        {
            public void Configure(EntityTypeBuilder<Stock> builder)
            {
                builder.HasOne(model => model.Branch)
                    .WithMany(branch => branch.IngredientStocks)
                    .HasForeignKey(model => new { model.RestaurantId, model.BranchId })
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(model => model.Ingredient)
                    .WithMany()
                    .HasForeignKey(model => new { model.RestaurantId, model.IngredientId })
                    .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}