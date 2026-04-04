namespace FoodSphere.Common.Entity;

public interface IBranchStaffKey
{
    public Guid RestaurantId { get; }
    public short BranchId { get; }
    public string MasterId { get; }
}

public record BranchStaffKey(Guid RestaurantId, short BranchId, string MasterId) : IBranchStaffKey, IEntityKey
{
    public static implicit operator BranchStaffKey(BranchStaff model) => new(model.RestaurantId, model.BranchId, model.MasterId);
    public static implicit operator object[](BranchStaffKey key) => [key.RestaurantId, key.BranchId, key.MasterId];
}

public class BranchStaff : IBranchStaffKey, IUpdatableEntityModel, ISoftDeleteEntityModel
{
    public required Guid RestaurantId { get; init; }
    public required short BranchId { get; init; }
    public required string MasterId { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
    public DateTime? DeleteTime { get; set; }

    public virtual Branch Branch { get; init; } = null!;
    public virtual MasterUser Master { get; init; } = null!;

    public virtual ICollection<BranchStaffRole> Roles { get; } = [];

    public required string DisplayName { get; set; }
}