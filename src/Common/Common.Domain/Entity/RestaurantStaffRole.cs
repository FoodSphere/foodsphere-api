namespace FoodSphere.Common.Entity;

public interface IRestaurantStaffRoleKey
{
    public Guid RestaurantId { get; }
    public string MasterId { get; }
    public short RoleId { get; }
}

public record RestaurantStaffRoleKey(Guid RestaurantId, string MasterId, short RoleId) : IRestaurantStaffRoleKey, IEntityKey
{
    public static implicit operator RestaurantStaffRoleKey(RestaurantStaffRole model) => new(model.RestaurantId, model.MasterId, model.RoleId);
    public static implicit operator object[](RestaurantStaffRoleKey key) => [key.RestaurantId, key.MasterId, key.RoleId];
}

public class RestaurantStaffRole : IRestaurantStaffRoleKey, IUpdatableEntityModel
{
    public required Guid RestaurantId { get; init; }
    public required string MasterId { get; init; }
    public required short RoleId { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    public virtual RestaurantStaff Staff { get; init; } = null!;
    public virtual Role Role { get; init; } = null!;
}