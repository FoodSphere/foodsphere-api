using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodSphere.Data.Models
{
    [PrimaryKey(nameof(RestaurantId), nameof(MenuId), nameof(IngredientId))]
    public class MenuIngredient : TrackableModel
    {
        public Guid RestaurantId { get; set; }

        public short MenuId { get; set; }
        public virtual Menu Menu { get; set; } = null!;

        public short IngredientId { get; set; }
        public virtual Ingredient Ingredient { get; set; } = null!;

        [Column(TypeName = "numeric(10,4)")]
        public decimal Amount { get; set; }
    }

    namespace Configurations
    {
        public class MenuIngredientConfiguration : IEntityTypeConfiguration<MenuIngredient>
        {
            public void Configure(EntityTypeBuilder<MenuIngredient> builder)
            {
                builder.HasOne(model => model.Menu)
                    .WithMany(menu => menu.MenuIngredients)
                    .HasForeignKey(model => new { model.RestaurantId, model.MenuId })
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(model => model.Ingredient)
                    .WithMany(ingredient => ingredient.MenuIngredients)
                    .HasForeignKey(model => new { model.RestaurantId, model.IngredientId })
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}