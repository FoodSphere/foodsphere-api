namespace FoodSphere.Common.Entity;

public interface IOrderItemKey
{
    public Guid BillId { get; }
    public short OrderId { get; }
    public short Id { get; }
}

public record OrderItemKey(Guid BillId, short OrderId, short Id) : IOrderItemKey, IEntityKey
{
    public static implicit operator OrderItemKey(OrderItem model) => new(model.BillId, model.OrderId, model.Id);
    public static implicit operator object[](OrderItemKey key) => [key.BillId, key.OrderId, key.Id];
}

// OrderLineItem
public class OrderItem : IOrderItemKey, IUpdatableEntityModel, ISoftDeleteEntityModel
{
    public required Guid BillId { get; init; }
    public required short OrderId { get; init; }
    public required short Id { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
    public DateTime? DeleteTime { get; set; }

    public virtual Bill Bill { get; init; } = null!;
    public virtual Order Order { get; init; } = null!;

    public required Guid RestaurantId { get; init; }
    public required short MenuId { get; init; }
    public virtual Menu Menu { get; init; } = null!;

    public required string NameSnapshot { get; set; }
    public required int PriceSnapshot { get; set; }

    public required short Quantity { get; set; }
    public string? Note { get; set; }
}