namespace FoodSphere.Core.Entities;

public class Table : EntityBase<short>
{
    public Guid RestaurantId { get; set; }
    public short BranchId { get; set; }
    public virtual Branch Branch { get; set; } = null!;

    public string? Name { get; set; }
    public TableStatus Status { get; set; }
}