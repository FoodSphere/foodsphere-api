namespace FoodSphere.Core.Entities;

public class Ingredient : EntityBase<short>
{
    public Guid RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; set; } = null!;

    public virtual List<MenuIngredient> MenuIngredients { get; } = [];
    public virtual List<IngredientTag> IngredientTags { get; } = [];

    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Unit { get; set; }
    public IngredientStatus Status { get; set; }
}

public class IngredientTag: TrackableEntityBase
{
    public Guid RestaurantId { get; set; }
    public short IngredientId { get; set; }
    public virtual Ingredient Ingredient { get; set; } = null!;

    public required string TagId { get; set; }
    public virtual Tag Tag { get; set; } = null!;
}