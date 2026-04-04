namespace FoodSphere.Common.Entity;

public interface IMenuTagKey
{
    public Guid RestaurantId { get; }
    public short MenuId { get; }
    public short TagId { get; }
}

public record MenuTagKey(Guid RestaurantId, short MenuId, short TagId) : IMenuTagKey, IEntityKey
{
    public static implicit operator MenuTagKey(TagMenu model) => new(model.RestaurantId, model.MenuId, model.TagId);
    public static implicit operator object[](MenuTagKey key) => [key.RestaurantId, key.MenuId, key.TagId];
}

public class TagMenu : IMenuTagKey, IUpdatableEntityModel
{
    public required Guid RestaurantId { get; init; }
    public required short MenuId { get; init; }
    public required short TagId { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    public virtual Menu Menu { get; init; } = null!;
    public virtual Tag Tag { get; init; } = null!;
}