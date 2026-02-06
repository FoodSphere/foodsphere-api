namespace FoodSphere.Common.Entity;

public class Queuing : EntityBase<string>
{
    public Guid RestaurantId { get; set; }
    public short BranchId { get; set; }
    public virtual Branch Branch { get; set; } = null!;

    public Guid? ConsumerId { get; set; }
    public virtual ConsumerUser? Consumer { get; set; } = null!;

    public short? Pax { get; set; }
}