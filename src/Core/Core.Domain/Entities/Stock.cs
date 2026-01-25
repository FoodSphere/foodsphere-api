namespace FoodSphere.Core.Entities;

public class Stock : TrackableEntityBase
{
    public Guid RestaurantId { get; set; }

    public short BranchId { get; set; }
    public virtual Branch Branch { get; set; } = null!;

    public short IngredientId { get; set; }
    public virtual Ingredient Ingredient { get; set; } = null!;

    public decimal Amount { get; set; }

    public StockStatus Status { get; set; }
}