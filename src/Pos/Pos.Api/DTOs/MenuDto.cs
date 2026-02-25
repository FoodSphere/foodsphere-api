namespace FoodSphere.Pos.Api.DTO;

public class IngredientItemDto
{
    public short ingredient_id { get; set; }

    /// <example>0.25</example>
    public decimal amount { get; set; }

    public static IngredientItemDto FromModel(MenuIngredient model)
    {
        return new IngredientItemDto
        {
            ingredient_id = model.IngredientId,
            amount = model.Amount,
        };
    }
}

public class MenuRequest
{
    /// <example>ข้าวผัดโคขุน</example>
    public required string name { get; set; }

    /// <example>120</example>
    public int price { get; set; }

    public List<IngredientItemDto> ingredients { get; set; } = [];

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

    public IngredientItemDto[] ingredients { get; set; } = [];
    public TagDto[] tags { get; set; } = [];

    /// <example>ข้าวผัดโคขุน</example>
    public required string name { get; set; }

    /// <example>ข้าวผัดโคขุนสูตรเด็ดของทางร้าน</example>
    public string? display_name { get; set; }

    /// <example>เนื้อโคจากออสเตรเลีย</example>
    public string? description { get; set; }

    public string? image_url { get; set; }

    /// <example>120</example>
    public int price { get; set; }

    public MenuStatus status { get; set; }

    public static MenuResponse FromModel(Menu model)
    {
        return new MenuResponse
        {
            id = model.Id,
            create_time = model.CreateTime,
            update_time = model.UpdateTime,
            restaurant_id = model.RestaurantId,
            ingredients = [.. model.MenuIngredients.Select(IngredientItemDto.FromModel)],
            tags = [.. model.MenuTags.Select(TagDto.FromModel)],
            name = model.Name,
            display_name = model.DisplayName,
            description = model.Description,
            image_url = model.ImageUrl,
            price = model.Price,
            status = model.Status,
        };
    }
}