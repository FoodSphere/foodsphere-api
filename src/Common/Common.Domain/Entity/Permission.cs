namespace FoodSphere.Common.Entity;

public interface IPermissionKey
{
    public int Id { get; }
}

public record PermissionKey(int Id) : IPermissionKey, IEntityKey
{
    public static implicit operator PermissionKey(Permission model) => new(model.Id);
    public static implicit operator object[](PermissionKey key) => [key.Id];
}

public class Permission : IPermissionKey, IUpdatableEntityModel, ISoftDeleteEntityModel
{
    public required int Id { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
    public DateTime? DeleteTime { get; set; }

    public required string Name { get; set; }
    public string? Description { get; set; }
}