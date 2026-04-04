namespace FoodSphere.Common.Entity;

public interface IRestaurantKey
{
    public Guid Id { get; }
}

public record RestaurantKey(Guid Id) : IRestaurantKey, IEntityKey
{
    public static implicit operator RestaurantKey(Restaurant model) => new(model.Id);
    public static implicit operator object[](RestaurantKey key) => [key.Id];
}

public class Restaurant : IRestaurantKey, IUpdatableEntityModel, ISoftDeleteEntityModel
{
    public required Guid Id { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
    public DateTime? DeleteTime { get; set; }

    public required string OwnerId { get; init; }
    public virtual MasterUser Owner { get; init; } = null!;

    public Contact Contact { get; set; } = new();

    public virtual ICollection<RestaurantStaff> Staffs { get; } = [];
    public virtual ICollection<Menu> Menus { get; } = [];
    public virtual ICollection<Tag> Tags { get; } = [];
    public virtual ICollection<Ingredient> Ingredients { get; } = [];
    public virtual ICollection<Branch> Branches { get; } = [];

    public required string Name { get; set; }
    public string? StripeAccountId { get; set; }
    public string? DisplayName { get; set; }
    public string? ImageUrl { get; set; }
}