namespace FoodSphere.Common.Entity;

public interface IBranchStaffRoleKey
{
    public Guid RestaurantId { get; }
    public short BranchId { get; }
    public string MasterId { get; }
    public short RoleId { get; }
}

public record BranchStaffRoleKey(Guid RestaurantId, short BranchId, string MasterId, short RoleId) : IBranchStaffRoleKey, IEntityKey
{
    public static implicit operator BranchStaffRoleKey(BranchStaffRole model) => new(model.RestaurantId, model.BranchId, model.MasterId, model.RoleId);
    public static implicit operator object[](BranchStaffRoleKey key) => [key.RestaurantId, key.BranchId, key.MasterId, key.RoleId];
}

public class BranchStaffRole : IBranchStaffRoleKey, IUpdatableEntityModel
{
    public required Guid RestaurantId { get; init; }
    public required short BranchId { get; init; }
    public required string MasterId { get; init; }
    public required short RoleId { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    public virtual BranchStaff Staff { get; init; } = null!;
    public virtual Role Role { get; init; } = null!;
}