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

    public virtual List<Menu> Menus { get; } = [];
    public virtual List<Tag> Tags { get; } = [];
    public virtual List<Ingredient> Ingredient { get; } = [];
    public virtual List<Branch> Branches { get; } = [];

    public required string Name { get; set; }
    public string? DisplayName { get; set; }
}

public class Tag : TrackableEntityBase
{
    public Guid RestaurantId { get; set; }
    public required string Name { get; set; }

    public virtual List<MenuTag> MenuTags { get; } = [];
    public virtual List<IngredientTag> IngredientTags { get; } = [];
}