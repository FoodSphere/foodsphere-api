namespace FoodSphere.Common.Entity;

public interface IBillKey
{
    public Guid Id { get; }
}

public record BillKey(Guid Id) : IBillKey, IEntityKey
{
    public static implicit operator BillKey(Bill model) => new(model.Id);
    public static implicit operator object[](BillKey key) => [key.Id];
}

public class Bill : IBillKey, IUpdatableEntityModel
{
    public required Guid Id { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    public required Guid RestaurantId { get; init; }
    public required short BranchId { get; init; }

    public required short TableId { get; set; }
    public virtual Table Table { get; set; } = null!;

    public short? IssuerId { get; set; }
    public virtual WorkerUser? Issuer { get; set; }

    public Guid? ConsumerId { get; set; }
    public virtual ConsumerUser? Consumer { get; set; }

    public virtual ICollection<BillMember> Members { get; } = [];
    public virtual ICollection<OrderingPortal> Portals { get; } = [];
    public virtual ICollection<Order> Orders { get; } = [];
    public virtual ICollection<Payment> Payments { get; } = [];

    public short? Pax { get; set; }
    public BillStatus Status { get; set; }
}