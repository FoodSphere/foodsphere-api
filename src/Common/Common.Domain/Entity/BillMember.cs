namespace FoodSphere.Common.Entity;

public interface IBillMemberKey
{
    public Guid BillId { get; }
    public short Id { get; }
}

public record BillMemberKey(Guid BillId, short Id) : IBillMemberKey, IEntityKey
{
    public static implicit operator BillMemberKey(BillMember model) => new(model.BillId, model.Id);
    public static implicit operator object[](BillMemberKey key) => [key.BillId, key.Id];
}

public class BillMember : IBillMemberKey, IUpdatableEntityModel
{
    public required Guid BillId { get; init; }
    public required short Id { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    public virtual Bill Bill { get; init; } = null!;

    public Guid? ConsumerId { get; set; }
    public virtual ConsumerUser? Consumer { get; init; }

    public virtual ICollection<Order> Orders { get; } = [];

    public string? Name { get; set; }
}