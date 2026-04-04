namespace FoodSphere.Common.Entity;

public interface IMenuComponentKey
{
    public Guid RestaurantId { get; }
    public short ParentMenuId { get; }
    public short ChildMenuId { get; }
}

public record MenuComponentKey(Guid RestaurantId, short ParentMenuId, short ChildMenuId) : IMenuComponentKey, IEntityKey
{
    public static implicit operator MenuComponentKey(MenuComponent model) => new(model.RestaurantId, model.ParentMenuId, model.ChildMenuId);
    public static implicit operator object[](MenuComponentKey key) => [key.RestaurantId, key.ParentMenuId, key.ChildMenuId];
}

public class MenuComponent : IMenuComponentKey, IUpdatableEntityModel
{
    public required Guid RestaurantId { get; init; }
    public required short ParentMenuId { get; init; }
    public required short ChildMenuId { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    public virtual Restaurant Restaurant { get; init; } = null!;
    public virtual Menu ParentMenu { get; init; } = null!;
    public virtual Menu ChildMenu { get; init; } = null!;

    public required short Quantity { get; set; }
}