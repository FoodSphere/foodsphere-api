namespace FoodSphere.Common.Entity;

public interface IIngredientTagKey
{
    public Guid RestaurantId { get; }
    public short IngredientId { get; }
    public short TagId { get; }
}

public record IngredientTagKey(Guid RestaurantId, short IngredientId, short TagId) : IIngredientTagKey, IEntityKey
{
    public static implicit operator IngredientTagKey(TagIngredient model) => new(model.RestaurantId, model.IngredientId, model.TagId);
    public static implicit operator object[](IngredientTagKey key) => [key.RestaurantId, key.IngredientId, key.TagId];
}

public class TagIngredient: IIngredientTagKey, IUpdatableEntityModel
{
    public required Guid RestaurantId { get; init; }
    public required short IngredientId { get; init; }
    public required short TagId { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    public virtual Ingredient Ingredient { get; init; } = null!;
    public virtual Tag Tag { get; init; } = null!;
}