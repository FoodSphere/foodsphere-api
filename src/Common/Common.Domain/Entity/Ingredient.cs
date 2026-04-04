namespace FoodSphere.Common.Entity;

public interface IIngredientKey
{
    public Guid RestaurantId { get; }
    public short Id { get; }
}

public record IngredientKey(Guid RestaurantId, short Id) : IIngredientKey, IEntityKey
{
    public static implicit operator IngredientKey(Ingredient model) => new(model.RestaurantId, model.Id);
    public static implicit operator object[](IngredientKey key) => [key.RestaurantId, key.Id];
}

public class Ingredient : IIngredientKey, IUpdatableEntityModel, ISoftDeleteEntityModel
{
    public required Guid RestaurantId { get; init; }
    public required short Id { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
    public DateTime? DeleteTime { get; set; }

    public virtual Restaurant Restaurant { get; init; } = null!;

    public virtual ICollection<MenuIngredient> MenuIngredients { get; } = [];
    public virtual ICollection<TagIngredient> Tags { get; } = [];

    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Unit { get; set; }
    public IngredientStatus Status { get; set; }
}