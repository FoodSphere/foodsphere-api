namespace FoodSphere.Pos.Api.DTO;

public class IngredientItemRequest
{
    /// <example>0.25</example>
    public decimal amount { get; set; }
}

public class IngredientItemDto
{
    public short ingredient_id { get; set; }

    /// <example>0.25</example>
    public decimal amount { get; set; }

    public static readonly Func<MenuIngredient, IngredientItemDto> Project = Projection.Compile();

    public static Expression<Func<MenuIngredient, IngredientItemDto>> Projection =>
        model => new IngredientItemDto
        {
            ingredient_id = model.IngredientId,
            amount = model.Amount,
        };
}

public class MenuIngredientResponse
{
    public short id { get; set; }

    /// <example>เนื้อโคขุน</example>
    public required string name { get; set; }

    /// <example>กิโล</example>
    public string? unit { get; set; }

    public string? image_url { get; set; }

    public static readonly Func<Ingredient, MenuIngredientResponse> Project = Projection.Compile();

    public static Expression<Func<Ingredient, MenuIngredientResponse>> Projection =>
        model => new MenuIngredientResponse
        {
            id = model.Id,
            name = model.Name,
            unit = model.Unit,
            image_url = model.ImageUrl,
        };
}

public class IngredientItemResponse
{
    public required MenuIngredientResponse ingredient { get; set; }

    /// <example>0.25</example>
    public decimal amount { get; set; }

    public static readonly Func<MenuIngredient, IngredientItemResponse> Project = Projection.Compile();

    public static Expression<Func<MenuIngredient, IngredientItemResponse>> Projection =>
        model => new IngredientItemResponse
        {
            ingredient = MenuIngredientResponse.Projection.Invoke(model.Ingredient),
            amount = model.Amount,
        };
}

public class MenuRequest
{
    /// <example>ข้าวผัดโคขุน</example>
    public required string name { get; set; }

    /// <example>120</example>
    public int price { get; set; }

    public IReadOnlyCollection<IngredientItemDto> ingredients { get; set; } = [];
    public IReadOnlyCollection<AssignTagRequest> tags { get; set; } = [];

    /// <example>ข้าวผัดโคขุนสูตรเด็ดของทางร้าน</example>
    public string? display_name { get; set; }

    /// <example>เนื้อโคจากออสเตรเลีย</example>
    public string? description { get; set; }
}

public class MenuResponse
{
    public int id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }

    /// <example>ข้าวผัดโคขุน</example>
    public required string name { get; set; }

    /// <example>120</example>
    public int price { get; set; }

    public IReadOnlyCollection<IngredientItemResponse> ingredients { get; set; } = [];
    public IReadOnlyCollection<TagDto> tags { get; set; } = [];

    /// <example>ข้าวผัดโคขุนสูตรเด็ดของทางร้าน</example>
    public string? display_name { get; set; }

    /// <example>เนื้อโคจากออสเตรเลีย</example>
    public string? description { get; set; }

    public string? image_url { get; set; }

    public MenuStatus status { get; set; }

    public static readonly Func<Menu, MenuResponse> Project = Projection.Compile();

    public static Expression<Func<Menu, MenuResponse>> Projection =>
        model => new MenuResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            restaurant_id = model.RestaurantId,
            name = model.Name,
            price = model.Price,
            ingredients = model.MenuIngredients
                .Select(m => IngredientItemResponse.Projection.Invoke(m))
                .ToArray(),
            tags = model.MenuTags
                .Select(m => TagDto.MenuTagProjection.Invoke(m))
                .ToArray(),
            display_name = model.DisplayName,
            description = model.Description,
            image_url = model.ImageUrl,
            status = model.Status,
        };
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