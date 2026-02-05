namespace FoodSphere.Common.Entities;

public class Role : EntityBase<short>
{
    public Guid RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; set; } = null!;

    public required string Name { get; set; }
    public string? Description { get; set; }

    public virtual List<RolePermission> Permissions { get; } = [];
}

public class RolePermission : TrackableEntityBase
{
    public Guid RestaurantId { get; set; }
    public short RoleId { get; set; }
    public virtual Role Role { get; set; } = null!;

    public int PermissionId { get; set; }
    public virtual Permission Permission { get; set; } = null!;
}

public class Permission : EntityBase<int>
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}