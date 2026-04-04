namespace FoodSphere.Common.Entity;

public interface IRestaurantStaffKey
{
    public Guid RestaurantId { get; }
    public string MasterId { get; }
}

public record RestaurantStaffKey(Guid RestaurantId, string MasterId) : IRestaurantStaffKey, IEntityKey
{
    public static implicit operator RestaurantStaffKey(RestaurantStaff model) => new(model.RestaurantId, model.MasterId);
    public static implicit operator object[](RestaurantStaffKey key) => [key.RestaurantId, key.MasterId];
}

public class RestaurantStaff : IRestaurantStaffKey, IUpdatableEntityModel, ISoftDeleteEntityModel
{
    public required Guid RestaurantId { get; init; }
    public required string MasterId { get; init; }

    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
    public DateTime? DeleteTime { get; set; }

    public virtual Restaurant Restaurant { get; init; } = null!;
    public virtual MasterUser Master { get; init; } = null!;

    public virtual ICollection<RestaurantStaffRole> Roles { get; } = [];

    public required string DisplayName { get; set; }
}