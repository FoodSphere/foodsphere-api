namespace FoodSphere.Resource.Api.DTO;

public class MenuRequest
{
    public List<MenuIngredientDto> ingredients { get; set; } = [];

    public required string name { get; set; }
    public string? display_name { get; set; }
    public string? description { get; set; }
    public string? image_url { get; set; }
    public short price { get; set; }
}

public class MenuResponse
{
    public int id { get; set; }

    public DateTime create_time { get; set; }
    public DateTime update_time { get; set; }

    public Guid restaurant_id { get; set; }

    public List<MenuIngredientDto> ingredients { get; set; } = [];

    public required string name { get; set; }
    public string? display_name { get; set; }
    public string? description { get; set; }
    public string? image_url { get; set; }
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
            ingredients = [.. model.MenuIngredients.Select(MenuIngredientDto.FromModel)],
            name = model.Name,
            display_name = model.DisplayName,
            description = model.Description,
            image_url = model.ImageUrl,
            price = model.Price,
            status = model.Status,
        };
    }
}

public class MenuIngredientDto
{
    public short ingredient_id { get; set; }
    public decimal amount { get; set; }

    public static MenuIngredientDto FromModel(MenuIngredient model)
    {
        return new MenuIngredientDto
        {
            ingredient_id = model.IngredientId,
            amount = model.Amount,
        };
    }
}