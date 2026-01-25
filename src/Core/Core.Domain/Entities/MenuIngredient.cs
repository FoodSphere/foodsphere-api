namespace FoodSphere.Core.Entities;

public class MenuIngredient : TrackableEntityBase
{
    public Guid RestaurantId { get; set; }

    public short MenuId { get; set; }
    public virtual Menu Menu { get; set; } = null!;

    public short IngredientId { get; set; }
    public virtual Ingredient Ingredient { get; set; } = null!;

    public decimal Amount { get; set; }
}