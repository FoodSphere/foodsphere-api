namespace FoodSphere.SelfOrdering.Api.DTO;

public record MenuComponentItemDto
{
    public required short menu_id { get; set; }
    public required short quantity { get; set; }

    public static readonly Expression<Func<MenuComponent, MenuComponentItemDto>> Projection =
        model => new()
        {
            menu_id = model.ChildMenuId,
            quantity = model.Quantity,
        };

    public static readonly Func<MenuComponent, MenuComponentItemDto> Project = Projection.Compile();
}

public record TagDto
{
    public required short id { get; set; }

    /// <example>meat</example>
    public required string name { get; set; }

    public static readonly Expression<Func<Tag, TagDto>> Projection =
        model => new()
        {
            id = model.Id,
            name = model.Name,
        };

    public static readonly Func<Tag, TagDto> Project = Projection.Compile();

    public static readonly Expression<Func<TagMenu, TagDto>> MenuTagProjection =
        model => new()
        {
            id = model.TagId,
            name = model.Tag.Name,
        };

    public static readonly Func<TagMenu, TagDto> MenuTagProject = MenuTagProjection.Compile();
}

public record MenuResponse
{
    public required int id { get; set; }

    /// <example>ข้าวผัดโคขุน</example>
    public required string name { get; set; }

    /// <example>120</example>
    public required int price { get; set; }

    public ICollection<MenuComponentItemDto> components { get; set; } = [];
    public ICollection<TagDto> tags { get; set; } = [];

    /// <example>ข้าวผัดโคขุนสูตรเด็ดของทางร้าน</example>
    public string? display_name { get; set; }

    /// <example>เนื้อโคจากออสเตรเลีย</example>
    public string? description { get; set; }

    public string? image_url { get; set; }

    public MenuStatus status { get; set; }

    public static readonly Expression<Func<Menu, MenuResponse>> Projection =
        model => new()
        {
            id = model.Id,
            name = model.Name,
            display_name = model.DisplayName,
            description = model.Description,
            image_url = model.ImageUrl,
            price = model.Price,
            components = model.Components
                .Select(m => MenuComponentItemDto.Projection.Invoke(m))
                .ToArray(),
            tags = model.Tags
                .Select(m => TagDto.MenuTagProjection.Invoke(m))
                .ToArray(),
            status = model.Status,
        };

    public static readonly Func<Menu, MenuResponse> Project = Projection.Compile();
}