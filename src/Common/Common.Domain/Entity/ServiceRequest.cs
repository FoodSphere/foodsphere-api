namespace FoodSphere.Common.Entity;

public interface IServiceRequestKey
{
    public Guid BillId { get; }
    public Guid Id { get; }
}

public record ServiceRequestKey(Guid BillId, Guid Id) : IServiceRequestKey, IEntityKey
{
    public static implicit operator ServiceRequestKey(ServiceRequest model) => new(model.BillId, model.Id);
    public static implicit operator object[](ServiceRequestKey key) => [key.BillId, key.Id];
}

public class ServiceRequest : IServiceRequestKey, IUpdatableEntityModel
{
    public required Guid BillId { get; init; }
    public required Guid Id { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    public virtual Bill Bill { get; init; } = null!;

    public required string Reason { get; set; }

    public ServiceRequestStatus Status { get; set; }
}