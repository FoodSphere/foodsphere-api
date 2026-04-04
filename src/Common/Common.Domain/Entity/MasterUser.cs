using Microsoft.AspNetCore.Identity;

namespace FoodSphere.Common.Entity;

public interface IMasterUserKey
{
    public string Id { get; }
}

public record MasterUserKey(string Id) : IMasterUserKey, IEntityKey
{
    public static implicit operator MasterUserKey(MasterUser model) => new(model.Id);
    public static implicit operator object[](MasterUserKey key) => [key.Id];
}

public class MasterUser : IdentityUser, IMasterUserKey
{
    public virtual ICollection<Restaurant> OwnedRestaurants { get; } = [];
    public virtual ICollection<RestaurantStaff> ManagedRestaurants { get; } = [];
    public virtual ICollection<BranchStaff> ManagedBranches { get; } = [];
}