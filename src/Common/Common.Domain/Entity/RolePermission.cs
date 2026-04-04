namespace FoodSphere.Common.Entity;

public interface IRolePermissionKey
{
    public Guid RestaurantId { get; }
    public short RoleId { get; }
    public int PermissionId { get; }
}

public record RolePermissionKey(Guid RestaurantId, short RoleId, int PermissionId) : IRolePermissionKey, IEntityKey
{
    public static implicit operator RolePermissionKey(RolePermission model) => new(model.RestaurantId, model.RoleId, model.PermissionId);
    public static implicit operator object[](RolePermissionKey key) => [key.RestaurantId, key.RoleId, key.PermissionId];
}

public class RolePermission : IRolePermissionKey, IUpdatableEntityModel
{
    public required Guid RestaurantId { get; init; }
    public required short RoleId { get; init; }
    public required int PermissionId { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    public virtual Role Role { get; init; } = null!;
    public virtual Permission Permission { get; init; } = null!;
}