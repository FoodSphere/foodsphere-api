using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodSphere.Data.Models
{
    [PrimaryKey(nameof(RestaurantId), nameof(Id))]
    public class Ingredient : BaseModel<short>
    {
        public Guid RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; } = null!;

        public virtual List<MenuIngredient> MenuIngredients { get; } = [];
        public virtual List<IngredientTag> IngredientTags { get; } = [];

        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? Unit { get; set; }
        public IngredientStatus Status { get; set; }
    }

    [PrimaryKey(nameof(RestaurantId), nameof(IngredientId), nameof(TagId))]
    public class IngredientTag: TrackableModel
    {
        public Guid RestaurantId { get; set; }
        public short IngredientId { get; set; }
        public virtual Ingredient Ingredient { get; set; } = null!;

        public required string TagId { get; set; }
        public virtual Tag Tag { get; set; } = null!;
    }

    namespace Configurations
    {
        public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
        {
            public void Configure(EntityTypeBuilder<Ingredient> builder)
            {
            }
        }

        public class IngredientTagConfiguration : IEntityTypeConfiguration<IngredientTag>
        {
            public void Configure(EntityTypeBuilder<IngredientTag> builder)
            {
                builder.HasOne(model => model.Ingredient)
                    .WithMany(ingredient => ingredient.IngredientTags)
                    .HasForeignKey(model => new { model.RestaurantId, model.IngredientId })
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(model => model.Tag)
                    .WithMany(tag => tag.IngredientTags)
                    .HasForeignKey(model => new { model.RestaurantId, model.TagId })
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}