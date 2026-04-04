namespace FoodSphere.Common.Entity;

public interface ITableKey
{
    public Guid RestaurantId { get; }
    public short BranchId { get; }
    public short Id { get; }
}

public record TableKey(Guid RestaurantId, short BranchId, short Id) : ITableKey, IEntityKey
{
    public static implicit operator TableKey(Table model) => new(model.RestaurantId, model.BranchId, model.Id);
    public static implicit operator object[](TableKey key) => [key.RestaurantId, key.BranchId, key.Id];
}

public class Table : ITableKey, IUpdatableEntityModel, ISoftDeleteEntityModel
{
    public required Guid RestaurantId { get; init; }
    public required short BranchId { get; init; }
    public required short Id { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
    public DateTime? DeleteTime { get; set; }

    public virtual Branch Branch { get; init; } = null!;

    public string? Name { get; set; }
    public TableStatus Status { get; set; }
}