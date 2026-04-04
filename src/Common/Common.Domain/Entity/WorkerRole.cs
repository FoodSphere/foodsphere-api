namespace FoodSphere.Common.Entity;

public interface IWorkerRoleKey
{
    public Guid RestaurantId { get; }
    public short BranchId { get; }
    public short WorkerId { get; }
    public short RoleId { get; }
}

public record WorkerRoleKey(Guid RestaurantId, short BranchId, short WorkerId, short RoleId) : IWorkerRoleKey, IEntityKey
{
    public static implicit operator WorkerRoleKey(WorkerRole model) => new(model.RestaurantId, model.BranchId, model.WorkerId, model.RoleId);
    public static implicit operator object[](WorkerRoleKey key) => [key.RestaurantId, key.BranchId, key.WorkerId, key.RoleId];
}

public class WorkerRole : IWorkerRoleKey, IUpdatableEntityModel
{
    public required Guid RestaurantId { get; init; }
    public required short BranchId { get; init; }
    public required short WorkerId { get; init; }
    public required short RoleId { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    public virtual WorkerUser Worker { get; init; } = null!;
    public virtual Role Role { get; init; } = null!;
}