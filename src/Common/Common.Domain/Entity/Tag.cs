namespace FoodSphere.Common.Entity;

public interface ITagKey
{
    public Guid RestaurantId { get; }
    public short Id { get; }
}

public record TagKey(Guid RestaurantId, short Id) : ITagKey, IEntityKey
{
    public static readonly Expression<Func<Tag, object>> KeyExpression =
        model => new
        {
            model.RestaurantId,
            model.Id,
        };

    public static implicit operator TagKey(Tag model) => new(model.RestaurantId, model.Id);
    public static implicit operator object[](TagKey key) => [key.RestaurantId, key.Id];
}

public class Tag : ITagKey, IUpdatableEntityModel
{
    public required Guid RestaurantId { get; init; }
    public required short Id { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    public required string Name { get; set; }
    public string Type { get; set; } = TagType.Default;

    public virtual ICollection<TagMenu> TagMenus { get; } = [];
    public virtual ICollection<TagIngredient> TagIngredients { get; } = [];
}