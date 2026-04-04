namespace FoodSphere.Pos.Api.DTO;

public record IngredientItemRequest
{
    /// <example>0.25</example>
    public required decimal amount { get; set; }
}

public record IngredientItemDto
{
    public required short ingredient_id { get; set; }

    /// <example>0.25</example>
    public required decimal amount { get; set; }

    public static readonly Expression<Func<MenuIngredient, IngredientItemDto>> Projection =
        model => new()
        {
            ingredient_id = model.IngredientId,
            amount = model.Amount,
        };

    public static readonly Func<MenuIngredient, IngredientItemDto> Project = Projection.Compile();
}

public record MenuIngredientResponse
{
    public required short id { get; set; }

    /// <example>เนื้อโคขุน</example>
    public required string name { get; set; }

    /// <example>กิโล</example>
    public string? unit { get; set; }

    public string? image_url { get; set; }

    public IngredientStatus status { get; set; }

    public static readonly Expression<Func<Ingredient, MenuIngredientResponse>> Projection =
        model => new()
        {
            id = model.Id,
            name = model.Name,
            unit = model.Unit,
            image_url = model.ImageUrl,
            status = model.Status,
        };

    public static readonly Func<Ingredient, MenuIngredientResponse> Project = Projection.Compile();
}

public record IngredientItemResponse
{
    public required MenuIngredientResponse ingredient { get; set; }

    /// <example>0.25</example>
    public required decimal amount { get; set; }

    public static readonly Expression<Func<MenuIngredient, IngredientItemResponse>> Projection =
        model => new()
        {
            ingredient = MenuIngredientResponse.Projection.Invoke(model.Ingredient),
            amount = model.Amount,
        };

    public static readonly Func<MenuIngredient, IngredientItemResponse> Project = Projection.Compile();
}

public record MenuComponentItemRequest
{
    public required short menu_id { get; set; }
    public required short quantity { get; set; }
}

public record MenuComponentItemResponse
{
    public required short menu_id { get; set; }
    public required short quantity { get; set; }

    public required bool stock_availability { get; set; }
    public MenuStatus menu_status { get; set; }

    public static readonly Expression<Func<MenuComponent, MenuComponentItemResponse>> Projection =
        model => new()
        {
            menu_id = model.ChildMenuId,
            quantity = model.Quantity,
            menu_status = model.ChildMenu.Status,
            stock_availability = model.ChildMenu.Ingredients.All(m =>
                m.Ingredient.Status == IngredientStatus.Active),
        };

    public static readonly Func<MenuComponent, MenuComponentItemResponse> Project = Projection.Compile();
}

public record MenuRequest
{
    /// <example>ข้าวผัดโคขุน</example>
    public required string name { get; set; }

    /// <example>120</example>
    public required int price { get; set; }

    public ICollection<IngredientItemDto> ingredients { get; set; } = [];
    public ICollection<MenuComponentItemRequest> components { get; set; } = [];
    public ICollection<AssignTagRequest> tags { get; set; } = [];

    /// <example>ข้าวผัดโคขุนสูตรเด็ดของทางร้าน</example>
    public string? display_name { get; set; }

    /// <example>เนื้อโคจากออสเตรเลีย</example>
    public string? description { get; set; }

    public MenuStatus status { get; set; }
}

public record MenuResponse
{
    public required int id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime? update_time { get; set; }
    public DateTime? delete_time { get; set; }

    public required Guid restaurant_id { get; set; }

    /// <example>ข้าวผัดโคขุน</example>
    public required string name { get; set; }

    /// <example>120</example>
    public required int price { get; set; }

    public ICollection<IngredientItemResponse> ingredients { get; set; } = [];
    public ICollection<MenuComponentItemResponse> components { get; set; } = [];
    public ICollection<TagDto> tags { get; set; } = [];

    /// <example>ข้าวผัดโคขุนสูตรเด็ดของทางร้าน</example>
    public string? display_name { get; set; }

    /// <example>เนื้อโคจากออสเตรเลีย</example>
    public string? description { get; set; }

    public string? image_url { get; set; }

    public required bool stock_availability { get; set; }
    public MenuStatus status { get; set; }

    public static readonly Expression<Func<Menu, MenuResponse>> Projection =
        model => new()
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            delete_time = model.DeleteTime,
            restaurant_id = model.RestaurantId,
            name = model.Name,
            price = model.Price,
            ingredients = model.Ingredients
                .Select(m => IngredientItemResponse.Projection.Invoke(m))
                .ToArray(),
            components = model.Components
                .Select(m => MenuComponentItemResponse.Projection.Invoke(m))
                .ToArray(),
            tags = model.Tags
                .Select(m => TagDto.MenuTagProjection.Invoke(m))
                .ToArray(),
            display_name = model.DisplayName,
            description = model.Description,
            image_url = model.ImageUrl,
            stock_availability = model.Ingredients.All(m =>
                m.Ingredient.Status == IngredientStatus.Active),
            status = model.Status,
        };

    public static readonly Func<Menu, MenuResponse> Project = Projection.Compile();
}

public class IngredientItemRequestValidator : AbstractValidator<IngredientItemRequest>
{
    public IngredientItemRequestValidator()
    {
        RuleFor(x => x.amount)
            .GreaterThan(0);
    }
}

public class IngredientItemDtoValidator : AbstractValidator<IngredientItemDto>
{
    public IngredientItemDtoValidator()
    {
        RuleFor(x => x.amount)
            .GreaterThan(0);
    }
}

public class MenuRequestValidator : AbstractValidator<MenuRequest>
{
    public MenuRequestValidator()
    {
        RuleFor(x => x.price)
            .GreaterThanOrEqualTo(0);
    }
}