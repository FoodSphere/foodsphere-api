namespace FoodSphere.Common.Entity;

public interface IMenuIngredientKey
{
    public Guid RestaurantId { get; }
    public short MenuId { get; }
    public short IngredientId { get; }
}

public record MenuIngredientKey(Guid RestaurantId, short MenuId, short IngredientId) : IMenuIngredientKey, IEntityKey
{
    public static implicit operator MenuIngredientKey(MenuIngredient model) => new(model.RestaurantId, model.MenuId, model.IngredientId);
    public static implicit operator object[](MenuIngredientKey key) => [key.RestaurantId, key.MenuId, key.IngredientId];
}

public class MenuIngredient : IMenuIngredientKey, IUpdatableEntityModel
{
    public required Guid RestaurantId { get; init; }
    public required short MenuId { get; init; }
    public required short IngredientId { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    public virtual Menu Menu { get; init; } = null!;
    public virtual Ingredient Ingredient { get; init; } = null!;

    public required decimal Amount { get; set; }
}