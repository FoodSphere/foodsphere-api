namespace FoodSphere.Common.Entity;

public interface IOrderKey
{
    public Guid BillId { get; }
    public short Id { get; }
}

public record OrderKey(Guid BillId, short Id) : IOrderKey, IEntityKey
{
    public static implicit operator OrderKey(Order model) => new(model.BillId, model.Id);
    public static implicit operator object[](OrderKey key) => [key.BillId, key.Id];
}

public class Order : IOrderKey, IUpdatableEntityModel, ISoftDeleteEntityModel
{
    public required Guid BillId { get; init; }
    public required short Id { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
    public DateTime? DeleteTime { get; set; }

    public virtual Bill Bill { get; init; } = null!;

    // if waiter create order for consumer
    public Guid? RestaurantId { get; init; }
    public short? BranchId { get; init; }
    public short? IssuerId { get; init; }
    public virtual WorkerUser? Issuer { get; init; }

    // if consumer request order himself on mobile
    public short? BillMemberId { get; init; }
    public virtual BillMember? BillMember { get; init; }

    public virtual ICollection<OrderItem> Items { get; } = [];

    public OrderStatus Status { get; set; }
}