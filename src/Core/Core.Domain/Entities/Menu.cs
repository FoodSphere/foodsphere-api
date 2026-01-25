namespace FoodSphere.Core.Entities;

public class Menu : EntityBase<short>
{
    public Guid RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; set; } = null!;

    public virtual List<MenuIngredient> MenuIngredients { get; } = [];
    public virtual List<MenuTag> MenuTags { get; } = [];
    public virtual List<MenuComponent> Components { get; } = [];

    public required string Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    // may separate for each branch?
    public int Price { get; set; }
    public MenuStatus Status { get; set; }
}

public class MenuComponent : TrackableEntityBase
{
    public Guid RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; set; } = null!;

    public short ParentMenuId { get; set; }
    public virtual Menu ParentMenu { get; set; } = null!;

    public short ChildMenuId { get; set; }
    public virtual Menu ChildMenu { get; set; } = null!;

    public short Quantity { get; set; }
}

public class MenuTag : TrackableEntityBase
{
    public Guid RestaurantId { get; set; }
    public short MenuId { get; set; }
    public virtual Menu Menu { get; set; } = null!;

    public required string TagId { get; set; }
    public virtual Tag Tag { get; set; } = null!;
}