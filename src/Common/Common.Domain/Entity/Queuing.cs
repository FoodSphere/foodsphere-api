namespace FoodSphere.Common.Entity;

public interface IQueuingKey
{
    public Guid RestaurantId { get; }
    public short BranchId { get; }
    public Guid Id { get; }
}

public record QueuingKey(Guid RestaurantId, short BranchId, Guid Id) : IQueuingKey, IEntityKey
{
    public static implicit operator QueuingKey(Queuing model) => new(model.RestaurantId, model.BranchId, model.Id);
    public static implicit operator object[](QueuingKey key) => [key.RestaurantId, key.BranchId, key.Id];
}

public class Queuing : IQueuingKey, IUpdatableEntityModel
{
    public required Guid RestaurantId { get; init; }
    public required short BranchId { get; init; }
    public required Guid Id { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    public virtual Branch Branch { get; init; } = null!;

    public Guid? ConsumerId { get; set; }
    public virtual ConsumerUser? Consumer { get; set; } = null!;

    public required string Code { get; set; }
    public short? Pax { get; set; }
}