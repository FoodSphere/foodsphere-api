namespace FoodSphere.Common.Entities;

public class Contact : EntityBase
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}

public class Restaurant : EntityBase
{
    public string OwnerId { get; set; } = null!;
    public virtual MasterUser Owner { get; set; } = null!;

    public Guid ContactId { get; set; }
    public virtual Contact Contact { get; set; } = null!;

    public virtual List<RestaurantManager> Managers { get; } = [];
    public virtual List<Menu> Menus { get; } = [];
    public virtual List<Tag> Tags { get; } = [];
    public virtual List<Ingredient> Ingredient { get; } = [];
    public virtual List<Branch> Branches { get; } = [];

    public required string Name { get; set; }
    public string? DisplayName { get; set; }
}

public class RestaurantManager : TrackableEntityBase
{
    public Guid RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; set; } = null!;

    public string MasterId { get; set; } = null!;
    public virtual MasterUser Master { get; set; } = null!;

    public virtual List<RestaurantManagerRole> Roles { get; } = [];

    public Permission[] GetPermissions()
    {
        var permissions = Roles
            .SelectMany(r => r.Role.Permissions)
            .Select(rp => rp.Permission)
            .Distinct()
            .ToArray();

        return permissions;
    }
}

public class RestaurantManagerRole : TrackableEntityBase
{
    public Guid RestaurantId { get; set; }

    public string ManagerId { get; set; } = null!;
    public virtual RestaurantManager Manager { get; set; } = null!;

    public short RoleId { get; set; }
    public virtual Role Role { get; set; } = null!;
}

public class Tag : TrackableEntityBase
{
    public Guid RestaurantId { get; set; }
    public required string Name { get; set; }

    public virtual List<MenuTag> MenuTags { get; } = [];
    public virtual List<IngredientTag> IngredientTags { get; } = [];
}