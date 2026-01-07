using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodSphere.Data.Models
{
    public class Contact : BaseModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }

    public class Restaurant : BaseModel
    {
        public string OwnerId { get; set; } = null!;
        public virtual MasterUser Owner { get; set; } = null!;

        public Guid ContactId { get; set; }
        public virtual Contact Contact { get; set; } = null!;

        public virtual List<Menu> Menus { get; } = [];
        public virtual List<Tag> Tags { get; } = [];
        public virtual List<Ingredient> Ingredient { get; } = [];
        public virtual List<Branch> Branches { get; } = [];

        public required string Name { get; set; }
        public string? DisplayName { get; set; }
    }

    [PrimaryKey(nameof(RestaurantId), nameof(Name))]
    public class Tag : TrackableModel
    {
        public Guid RestaurantId { get; set; }
        public required string Name { get; set; }

        public virtual List<MenuTag> MenuTags { get; } = [];
        public virtual List<IngredientTag> IngredientTags { get; } = [];
    }

    namespace Configurations
    {
        public class ContactConfiguration : IEntityTypeConfiguration<Contact>
        {
            public void Configure(EntityTypeBuilder<Contact> builder)
            {
            }
        }

        public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
        {
            public void Configure(EntityTypeBuilder<Restaurant> builder)
            {
                builder.HasOne(model => model.Owner)
                    .WithMany(user => user.OwnedRestaurants)
                    .HasForeignKey(model => model.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }

        public class TagConfiguration : IEntityTypeConfiguration<Tag>
        {
            public void Configure(EntityTypeBuilder<Tag> builder)
            {
                builder.HasOne<Restaurant>()
                    .WithMany(restaurant => restaurant.Tags)
                    .HasForeignKey(model => model.RestaurantId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}