namespace FoodSphere.Common.Entity;

public interface IRoleKey
{
    public Guid RestaurantId { get; }
    public short Id { get; }
}

public record RoleKey(Guid RestaurantId, short Id) : IRoleKey, IEntityKey
{
    public static implicit operator RoleKey(Role model) => new(model.RestaurantId, model.Id);
    public static implicit operator object[](RoleKey key) => [key.RestaurantId, key.Id];
}

public class Role : IRoleKey, IUpdatableEntityModel
{
    public required Guid RestaurantId { get; init; }
    public required short Id { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    public virtual Restaurant Restaurant { get; init; } = null!;

    public required string Name { get; set; }
    public string? Description { get; set; }

    public virtual ICollection<RolePermission> Permissions { get; } = [];
}