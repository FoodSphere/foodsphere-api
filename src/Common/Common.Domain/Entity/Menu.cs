namespace FoodSphere.Common.Entity;

public interface IMenuKey
{
    public Guid RestaurantId { get; }
    public short Id { get; }
}

public record MenuKey(Guid RestaurantId, short Id) : IMenuKey, IEntityKey
{
    public static implicit operator MenuKey(Menu model) => new(model.RestaurantId, model.Id);
    public static implicit operator object[](MenuKey key) => [key.RestaurantId, key.Id];
}

public class Menu : IMenuKey, IUpdatableEntityModel, ISoftDeleteEntityModel
{
    public required Guid RestaurantId { get; init; }
    public required short Id { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
    public DateTime? DeleteTime { get; set; }

    public virtual Restaurant Restaurant { get; init; } = null!;

    public virtual ICollection<MenuIngredient> Ingredients { get; } = [];
    public virtual ICollection<TagMenu> Tags { get; } = [];
    public virtual ICollection<MenuComponent> Components { get; } = [];

    public required string Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    // may separate for each branch?
    public required int Price { get; set; }
    public MenuStatus Status { get; set; }
}