using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodSphere.Data.Models
{
    [PrimaryKey(nameof(RestaurantId), nameof(Id))]
    public class Menu : BaseModel<short>
    {
        public Guid RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; } = null!;

        public virtual List<MenuIngredient> MenuIngredients { get; } = [];
        public virtual List<MenuTag> MenuTags { get; } = [];
        public virtual List<MenuComponent> Components { get; } = [];

        public required string Name { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        // may separate for each branch?
        public int Price { get; set; }
        public MenuStatus Status { get; set; }
    }

    [PrimaryKey(nameof(RestaurantId), nameof(ParentMenuId), nameof(ChildMenuId))]
    public class MenuComponent : TrackableModel
    {
        public Guid RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; } = null!;

        public short ParentMenuId { get; set; }
        public virtual Menu ParentMenu { get; set; } = null!;

        public short ChildMenuId { get; set; }
        public virtual Menu ChildMenu { get; set; } = null!;

        public short Quantity { get; set; }
    }

    [PrimaryKey(nameof(RestaurantId), nameof(MenuId), nameof(TagId))]
    public class MenuTag : TrackableModel
    {
        public Guid RestaurantId { get; set; }
        public short MenuId { get; set; }
        public virtual Menu Menu { get; set; } = null!;

        public required string TagId { get; set; }
        public virtual Tag Tag { get; set; } = null!;
    }

    namespace Configurations
    {
        public class MenuConfiguration : IEntityTypeConfiguration<Menu>
        {
            public void Configure(EntityTypeBuilder<Menu> builder)
            {

            }
        }

        public class MenuComponentConfiguration : IEntityTypeConfiguration<MenuComponent>
        {
            public void Configure(EntityTypeBuilder<MenuComponent> builder)
            {
                builder.HasOne(model => model.ParentMenu)
                    .WithMany(menu => menu.Components)
                    .HasForeignKey(model => new { model.RestaurantId, model.ParentMenuId })
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(model => model.ChildMenu)
                    .WithMany()
                    .HasForeignKey(model => new { model.RestaurantId, model.ChildMenuId })
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class MenuTagConfiguration : IEntityTypeConfiguration<MenuTag>
        {
            public void Configure(EntityTypeBuilder<MenuTag> builder)
            {
                builder.HasOne(model => model.Menu)
                    .WithMany(menu => menu.MenuTags)
                    .HasForeignKey(model => new { model.RestaurantId, model.MenuId })
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(model => model.Tag)
                    .WithMany(tag => tag.MenuTags)
                    .HasForeignKey(model => new { model.RestaurantId, model.TagId })
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}