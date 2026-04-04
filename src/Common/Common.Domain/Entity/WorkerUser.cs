namespace FoodSphere.Common.Entity;

public interface IWorkerUserKey
{
    public Guid RestaurantId { get; }
    public short BranchId { get; }
    public short Id { get; }
}

public record WorkerUserKey(Guid RestaurantId, short BranchId, short Id) : IWorkerUserKey, IEntityKey
{
    public static implicit operator WorkerUserKey(WorkerUser model) => new(model.RestaurantId, model.BranchId, model.Id);
    public static implicit operator object[](WorkerUserKey key) => [key.RestaurantId, key.BranchId, key.Id];
}

public class WorkerUser : IWorkerUserKey, IUpdatableEntityModel, ISoftDeleteEntityModel
{
    public required Guid RestaurantId { get; init; }
    public required short BranchId { get; init; }
    public required short Id { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
    public DateTime? DeleteTime { get; set; }

    public virtual Branch Branch { get; init; } = null!;

    public required string Name { get; set; }
    public string? Position { get; set; }
    public string? Phone { get; set; }

    public virtual ICollection<WorkerRole> Roles { get; } = [];
}