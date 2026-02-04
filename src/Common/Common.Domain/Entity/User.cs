using Microsoft.AspNetCore.Identity;

namespace FoodSphere.Common.Entities;

public class MasterUser : IdentityUser
{
    public virtual List<Restaurant> OwnedRestaurants { get; } = [];
    public virtual List<Manager> ManagedBranches { get; } = [];
}

public class StaffUser : EntityBase<short>
{
    public Guid RestaurantId { get; set; }
    public short BranchId { get; set; }
    public virtual Branch Branch { get; set; } = null!;

    public required string Name { get; set; }
    public string? Position { get; set; }
    public string? Phone { get; set; }

    public virtual List<StaffRole> Roles { get; } = [];
}

public class StaffRole : TrackableEntityBase
{
    public Guid RestaurantId { get; set; }
    public short BranchId { get; set; }

    public short StaffId { get; set; }
    public virtual StaffUser Staff { get; set; } = null!;

    public short RoleId { get; set; }
    public virtual Role Role { get; set; } = null!;
}

public class ConsumerUser : EntityBase
{
    public virtual List<Bill> Bills { get; } = [];

    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public bool TwoFactorEnabled { get; set; } = false;

    public string GetSecurityStamp()
    {
        throw new NotImplementedException();
    }
}